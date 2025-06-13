using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "User";
    public List<TaskItem> Tasks { get; set; } = new();
}