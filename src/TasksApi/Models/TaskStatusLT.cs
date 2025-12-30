using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

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
    [EnumMember(Value = "None")]
    None = 1,
    [EnumMember(Value = "Created")]
    Created = 2,
    [EnumMember(Value = "In Progress")]
    InProgress=3,
    [EnumMember(Value = "Completed")]
    Completed=4,
    [EnumMember(Value = "Cancelled")]
    Cancelled=5
}