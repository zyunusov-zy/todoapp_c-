using TodoApp.DTOs;
using TodoApp.Models;
using TodoApp.Repositories;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto, int userId)
    {
        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            UserId = userId
        };

        var saved = await _repository.AddAsync(task);

        return new TaskResponseDto
        {
            Id = saved.Id,
            Title = saved.Title,
            Description = saved.Description,
            IsCompleted = saved.IsCompleted
        };
    }

    public async Task<List<TaskResponseDto>> GetAllTasksAsync(int userId)
    {
        var tasks = await _repository.GetAllByUserIdAsync(userId);
        return tasks.Select(t => new TaskResponseDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            IsCompleted = t.IsCompleted
        }).ToList();
    }

    public async Task<TaskResponseDto?> GetTaskByIdAsync(int taskId, int userId)
    {
        var task = await _repository.GetByIdAsync(taskId, userId);
        if (task == null) return null;

        return new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IsCompleted = task.IsCompleted
        };
    }

    public async Task UpdateTaskAsync(int taskId, UpdateTaskDto dto, int userId)
    {
        var task = await _repository.GetByIdAsync(taskId, userId);
        if (task == null) throw new Exception("Task not found");

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.IsCompleted = dto.IsCompleted;

        await _repository.UpdateAsync(task);
    }

    public async Task DeleteTaskAsync(int taskId, int userId)
    {
        var task = await _repository.GetByIdAsync(taskId, userId);
        if (task == null) throw new Exception("Task not found");

        await _repository.DeleteAsync(task);
    }
}
