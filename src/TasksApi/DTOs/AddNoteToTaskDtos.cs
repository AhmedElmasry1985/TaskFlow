using System.ComponentModel.DataAnnotations;
using Core;

namespace TasksApi.DTOs;

public class AddNoteToTaskRequestDto
{
    [Required(ErrorMessage = "Content is required")]
    [MaxLength(500, ErrorMessage = "Content cannot exceed 500 characters")]
    public string Content { get; set; }
    
    [Required(ErrorMessage = "TaskId is required")]
    public int TaskId { get; set; }
    
    [Required(ErrorMessage = "CreatorUserId is required")]
    public int CreatorUserId { get; set; }
}

public class AddNoteToTaskResponseDto : ResponseDtoBase
{
    public NoteDto Note { get; set; }
}
