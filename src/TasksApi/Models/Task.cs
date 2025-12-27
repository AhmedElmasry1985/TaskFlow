using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace TasksApi.Models;

public class Task
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]public int Id { get; set; }
    [Required,MaxLength(100)]public string Title { get; set; }
    [MaxLength(500)]public string Description { get; set; }
    [Required]public int StatusId { get; set; }
    public TaskStatusLT Status { get; set; }
    [Required]public int CreatorUserId { get; set; }
    public User CreatorUser { get; set; }
    [Required]public int AssignedUserId { get; set; }
    public User AssignedUser { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    public DateTime ModificationDate { get; set; } = DateTime.UtcNow;
    public Collection<Note> Notes { get; set; } = [];
}

