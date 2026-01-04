namespace NotificationService.DTOs;

public class PublishTaskStatusMessageDto:MessageDto
{
    public string Title { get; set; }
    public string OldStatus { get; set; }
    public string NewStatus { get; set; }
}