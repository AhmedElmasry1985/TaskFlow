namespace NotificationService.DTOs;

public class RegisterUserMessageDto:MessageDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
}