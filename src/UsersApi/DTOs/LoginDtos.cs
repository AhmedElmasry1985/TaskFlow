using Core;
using Core.RepositoryPattern;

namespace UsersApi.DTOs;

public class LoginRequestDto
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class LoginResponseDto:ResponseDtoBase
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    
}

public static class LoginRequestValidator
{
    public static ValidationResult Validate(this LoginRequestDto loginRequestDto)
    {
        var result = new ValidationResult();
         if(string.IsNullOrWhiteSpace(loginRequestDto.Password))
            result.Message = "HashedPassword is required";
        else if(string.IsNullOrWhiteSpace(loginRequestDto.Username))
            result.Message = "Username is required";
        else result.IsSuccess = true;
        return result;
    }
}