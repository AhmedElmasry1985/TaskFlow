namespace TasksApi.DTOs;

public class NoteDto
{
    public int Id { get; set; }
    public string Content { get; set; }
    public int TaskId { get; set; }
    public DateTime CreationDate { get; set; }
    public int CreatorUserId { get; set; }
    public string CreatorUsername { get; set; }
}