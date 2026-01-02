using Core.MessageClient;

namespace UsersApi.DTOs;

public class PublishUserDto:BasePublishDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; } 
    public string Username { get; set; }
}