using Core;
using Core.RepositoryPattern;

namespace TasksApi.DTOs;

public class GetTasksAssignedToUserRequestDto
{
    //public int UserId { get; set; }
}

public class GetTasksAssignedToUserResponseDto : ResponseDtoBase
{
    public List<TaskDto> Tasks { get; set; } = [];
}
