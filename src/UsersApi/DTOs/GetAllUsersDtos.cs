using Core;
using Core.RepositoryPattern;

namespace UsersApi.DTOs;

public class GetAllUsersResponseDto : ResponseDtoBase
{
    public List<UserDto> Users { get; set; } = new();
}

public class UserDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
}

public class GetCurrentUserResponseDto : ResponseDtoBase
{
    public UserDto User { get; set; }
}
