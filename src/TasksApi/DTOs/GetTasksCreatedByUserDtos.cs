using Core;

namespace TasksApi.DTOs;

public class GetTasksCreatedByUserRequestDto
{
    //public int UserId { get; set; }
}

public class GetTasksCreatedByUserResponseDto : ResponseDtoBase
{
    public List<TaskDto> Tasks { get; set; } = [];
}