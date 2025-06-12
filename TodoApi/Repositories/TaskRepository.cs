using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;


public class TaskRepository : ITaskRepository
{
    private readonly TodoAppDbContext _context;

    public TaskRepository(TodoAppDbContext context) => _context = context;

    public async Task<TaskItem> AddAsync(TaskItem task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public Task<List<TaskItem>> GetAllByUserIdAsync(int userId) =>
        _context.Tasks.Where(t => t.UserId == userId).ToListAsync();

    public Task<TaskItem?> GetByIdAsync(int id, int userId) =>
        _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    public async Task UpdateAsync(TaskItem task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TaskItem task)
    {
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
    }
}