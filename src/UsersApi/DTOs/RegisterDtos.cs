using Core;
using Core.RepositoryPattern;

namespace UsersApi.DTOs;

public class RegisterRequestDto
{
    public string FullName { get; set; }
    public string Password { get; set; }
    //user shouldn't specify role, but for simplicity we will allow it
    public string Role { get; set; } 
    public string Username { get; set; }

}

public class RegisterResponseDto:ResponseDtoBase
{
    public int UserId { get; set; }
}

public static class RegisterRequestValidator
{
    static string[] Roles = ["Admin", "User"];
    public static ValidationResult Validate(this RegisterRequestDto registerRequestDto)
    {
        var result = new ValidationResult();
        if(string.IsNullOrWhiteSpace(registerRequestDto.FullName))
            result.Message = "Full name is required";
        else if(string.IsNullOrWhiteSpace(registerRequestDto.Password))
            result.Message = "HashedPassword is required";
        else if(string.IsNullOrWhiteSpace(registerRequestDto.Role) || !Roles.Contains(registerRequestDto.Role))
            result.Message = "Role is required and should be Admin or User";
        else if(string.IsNullOrWhiteSpace(registerRequestDto.Username))
            result.Message = "Username is required";
        else result.IsSuccess = true;
        return result;
    }
}