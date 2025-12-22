using System.Security.Claims;
using System.Text.Json;
using Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersApi.Data;
using UsersApi.DTOs;
using UsersApi.Models;
using UsersApi.Services.Auth;
using UsersApi.Services.MessageBus;

namespace UsersApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserRepository userRepository, Jwt jwt, IMessageBusClient messageBusClient) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponseDto>> Register(RegisterRequestDto registerRequestDto)
    {
        var responseDto = new RegisterResponseDto();
        var validationResult = registerRequestDto.Validate();
        if (!validationResult.IsSuccess)
        {
            responseDto.Result = validationResult;
            return BadRequest(responseDto);
        }

        var isUserExists = await userRepository.GetUserByUsername(registerRequestDto.Username) != null;
        if (isUserExists)
        {
            responseDto.Result = new ValidationResult { Message = "Username already exists" };
            return BadRequest(responseDto);
        }

        var user = new User
        {
            FullName = registerRequestDto.FullName,
            HashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequestDto.Password),
            Role = registerRequestDto.Role,
            Username = registerRequestDto.Username,
        };
        await userRepository.Add(user);
        var isSaved = await userRepository.Save();
        if (!isSaved)
        {
            responseDto.Result = new ValidationResult { Message = "Failed to register user" };
            return BadRequest(responseDto);
        }

        responseDto.UserId = user.Id;
        responseDto.Result = new ValidationResult { IsSuccess = true, Message = "User registered successfully" };
        var publishUserDto = new PublishUserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Username = user.Username,
            Role = user.Role,
            Event = "UserRegistered",
            DateTime = DateTime.UtcNow
        };
        await messageBusClient.PublishMessage(JsonSerializer.Serialize(publishUserDto));
        return Ok(responseDto);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto loginRequestDto)
    {
        var responseDto = new LoginResponseDto();
        var validationResult = loginRequestDto.Validate();
        if (!validationResult.IsSuccess)
        {
            responseDto.Result = validationResult;
            return BadRequest(responseDto);
        }

        var user = await userRepository.GetUserByUsername(loginRequestDto.Username);
        if (user == null)
        {
            responseDto.Result = new ValidationResult { Message = "Username doesn't exist" };
            return BadRequest(responseDto);
        }

        var isPasswordMatched = BCrypt.Net.BCrypt.Verify(loginRequestDto.Password, user.HashedPassword);
        if (!isPasswordMatched)
        {
            responseDto.Result = new ValidationResult { Message = "Password doesn't match" };
            return BadRequest(responseDto);
        }

        responseDto.UserId = user.Id;
        responseDto.Username = user.Username;
        responseDto.Role = user.Role;
        responseDto.Token = jwt.GenerateToken(user);
        responseDto.ExpiresAt = DateTime.UtcNow.AddMinutes(Jwt.ExpiryMinutes ?? 30);
        responseDto.Result = new ValidationResult { IsSuccess = true, Message = "User logged in successfully" };
        return Ok(responseDto);
    }

    [Authorize]
    [HttpGet("all")]
    public async Task<ActionResult<GetAllUsersResponseDto>> GetAllUsers()
    {
        var responseDto = new GetAllUsersResponseDto();
        var users = await userRepository.FindByPredicate();
        responseDto.Users = users.Select(u => new UserDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Username = u.Username,
            Role = u.Role
        }).ToList();

        responseDto.Result = new ValidationResult { IsSuccess = true, Message = "Users retrieved successfully" };
        return Ok(responseDto);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<GetCurrentUserResponseDto>> GetCurrentUser()
    {
        var userIdFromClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var parseResult = int.TryParse(userIdFromClaim, out var userId);
        if (!parseResult)
            return BadRequest(new GetCurrentUserResponseDto
            {
                Result = new ValidationResult { IsSuccess = false, Message = "User not found" }
            });
        var user = await userRepository.FindById(userId);
        if (user == null)
            return BadRequest(new GetCurrentUserResponseDto
            {
                Result = new ValidationResult { IsSuccess = false, Message = "User not found" }
            });
        return Ok(new GetCurrentUserResponseDto
        {
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                Role = user.Role
            },
            Result = new ValidationResult { IsSuccess = true, Message = "User retrieved successfully" }
        });
    }
}