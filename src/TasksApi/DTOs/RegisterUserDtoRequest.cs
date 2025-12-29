namespace TasksApi.DTOs;

public class MessageDto
{
    public string EventName { get; set; }
    public string DateTime { get; set; }
    public string EventVersion { get; set; } = "v1"; // For backward compatibility

}
public class RegisterUserDtoRequest:MessageDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }

}
