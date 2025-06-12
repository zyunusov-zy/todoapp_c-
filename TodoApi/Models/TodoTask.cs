using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApp.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; } = false;

    [ForeignKey("User")]
    public int UserId { get; set; }
    public User? User { get; set; }
}