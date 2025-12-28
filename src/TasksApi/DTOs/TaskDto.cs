namespace TasksApi.DTOs;

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int StatusId { get; set; }
    public string StatusName { get; set; }
    public int CreatorUserId { get; set; }
    public string CreatorUsername { get; set; }
    public int AssignedUserId { get; set; }
    public string AssignedUsername { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModificationDate { get; set; }
    public List<NoteDto> Notes { get; set; } = [];
}

