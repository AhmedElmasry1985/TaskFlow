namespace UsersApi.DTOs;

public class PublishUserDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; } 
    public string Username { get; set; }
    // Create, Update, Delete
    public string Event { get; set; }
    public DateTime DateTime { get; set; }
}