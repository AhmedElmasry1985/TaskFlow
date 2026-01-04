namespace NotificationService.DTOs;

public class TaskCreatedMessageDto : MessageDto
{
    public string Title { get; set; }
    public string CreatorUsername { get; set; }
    public string AssignedUsername { get; set; }
    public DateTime CreationDate { get; set; }
}