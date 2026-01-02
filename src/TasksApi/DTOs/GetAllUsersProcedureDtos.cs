using Core;

namespace TasksApi.DTOs;

public class GetAllUsersProcedureDtoRequest
{
    
}

public class GetAllUsersProcedureDtoResponse : ResponseDtoBase
{
    public List<UsersProcedureDto> Users { get; set; } = [];
}

public class UsersProcedureDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
}