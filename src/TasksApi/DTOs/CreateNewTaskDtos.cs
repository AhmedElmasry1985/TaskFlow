using System.ComponentModel.DataAnnotations;
using Core;
using Core.RepositoryPattern;

namespace TasksApi.DTOs;

public class CreateNewTaskRequestDto
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    public string Title { get; set; }
    
    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; }
    
    // [Required(ErrorMessage = "CreatorUserId is required")]
    // public int CreatorUserId { get; set; }
    
    [Required(ErrorMessage = "AssignedUserId is required")]
    public int AssignedUserId { get; set; }
}

public class CreateNewTaskResponseDto : ResponseDtoBase
{
    public TaskDto Task { get; set; }
}
