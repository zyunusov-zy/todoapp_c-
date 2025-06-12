using TodoApp.DTOs;

public interface ITaskService
{
    Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto, int userId);
    Task<List<TaskResponseDto>> GetAllTasksAsync(int userId);
    Task<TaskResponseDto?> GetTaskByIdAsync(int taskId, int userId);
    Task UpdateTaskAsync(int taskId, UpdateTaskDto dto, int userId);
    Task DeleteTaskAsync(int taskId, int userId);
}
