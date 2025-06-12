using TodoApp.Models;

public interface ITaskRepository
{
    Task<TaskItem> AddAsync(TaskItem task);
    Task<List<TaskItem>> GetAllByUserIdAsync(int userId);
    Task<TaskItem?> GetByIdAsync(int id, int userId);
    Task UpdateAsync(TaskItem task);
    Task DeleteAsync(TaskItem task);
}