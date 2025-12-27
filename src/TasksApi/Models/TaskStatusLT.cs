using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TasksApi.Models;

public class TaskStatusLT
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required, MaxLength(15)] public string Name { get; set; }
    public Collection<Task> Tasks { get; set; } = [];
}

public enum TaskStatus
{
    None = 1,
    Created = 2,
    InProgress=3,
    Completed=4,
    Cancelled=5
}