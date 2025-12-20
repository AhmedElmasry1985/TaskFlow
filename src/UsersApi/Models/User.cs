using System.ComponentModel.DataAnnotations;

namespace UsersApi.Models;

public class User
{
    [Key] public int Id { get; set; }
    [Required,MaxLength(100)]public string FullName { get; set; }
    [Required,MaxLength(20)]public string HashedPassword { get; set; }
    // this should be related to another table, for now it's just a string: Admin, User
    [Required,MaxLength(20)]public string Role { get; set; } 
    [Required,MaxLength(20)]public string Username { get; set; }
    //no email for simplicity
}