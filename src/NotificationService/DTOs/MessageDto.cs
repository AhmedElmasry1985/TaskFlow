namespace NotificationService.DTOs;

public class MessageDto
{
    public string EventName { get; set; }
    public string DateTime { get; set; }
    public string EventVersion { get; set; } = "v1";
}