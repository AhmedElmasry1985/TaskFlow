namespace NotificationService.DTOs;

public class PublishNoteMessageDto:MessageDto
{
    public string Content { get; set; }
    public string TaskTitle { get; set; }
    public string CreatorUsername { get; set; }
    public DateTime CreationDate { get; set; }
}