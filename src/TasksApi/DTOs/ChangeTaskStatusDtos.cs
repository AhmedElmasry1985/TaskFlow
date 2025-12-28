using System.ComponentModel.DataAnnotations;
using Core;

namespace TasksApi.DTOs;

public class ChangeTaskStatusRequestDto
{
    [Required(ErrorMessage = "TaskId is required")]
    public int TaskId { get; set; }
    
    [Required(ErrorMessage = "NewStatusId is required")]
    [EnumDataType(typeof(Models.TaskStatus), ErrorMessage = "Invalid status value")]
    public int NewStatusId { get; set; }
}

public class ChangeTaskStatusResponseDto : ResponseDtoBase
{
    public TaskDto Task { get; set; }
}
