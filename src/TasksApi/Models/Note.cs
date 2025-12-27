using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TasksApi.Models;

public class Note
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]public int Id { get; set; }
    [Required,MaxLength(500)]public string Content { get; set; }
    [Required]public int TaskId { get; set; }
    public Task Task { get; set; }
    [Required]public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    [Required]public int CreatorUserId { get; set; }
    public User CreatorUser { get; set; }
}