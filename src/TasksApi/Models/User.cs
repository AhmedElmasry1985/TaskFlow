using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TasksApi.Models;

public class User
{
    [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]public int Id { get; set; }
    [Required,MaxLength(100)]public string FullName { get; set; }
    [Required,MaxLength(20)]public string Username { get; set; }
    [Required]public int ExternalId { get; set; }
    public Collection<Task> CreatedTasks { get; set; } = [];
    public Collection<Task> AssignedTasks { get; set; } = [];
    public Collection<Note> CreatedNotes { get; set; } = [];
}