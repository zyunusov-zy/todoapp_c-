using System.Data;
using Dapper;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Repositories;
public class TaskRepository : ITaskRepository
{
    private readonly IDbConnection _db;

    public TaskRepository(IDbConnection db) => _db = db;

    public async Task<TaskItem> AddAsync(TaskItem task)
    {
        var sql = @"
            INSERT INTO ""Tasks"" (Title, Description, IsCompleted, UserId)
            VALUES (@Title, @Description, @IsCompleted, @UserId)
            RETURNING Id;";

        task.Id = await _db.ExecuteScalarAsync<int>(sql, task);
        return task;
    }

    public async Task<List<TaskItem>> GetAllByUserIdAsync(int userId)
    {
        var sql = @"SELECT * FROM ""Tasks"" WHERE UserId = @UserId";
        var tasks = await _db.QueryAsync<TaskItem>(sql, new { UserId = userId });
        return tasks.ToList();
    }

    public async Task<TaskItem?> GetByIdAsync(int id, int userId)
    {
        var sql = @"SELECT * FROM ""Tasks"" WHERE Id = @Id AND UserId = @UserId";
        return await _db.QueryFirstOrDefaultAsync<TaskItem>(sql, new { Id = id, UserId = userId });
    }

    public async Task UpdateAsync(TaskItem task)
    {
        var sql = @"
            UPDATE ""Tasks""
            SET Title = @Title,
                Description = @Description,
                IsCompleted = @IsCompleted
            WHERE Id = @Id AND UserId = @UserId";

        await _db.ExecuteAsync(sql, task);
    }

    public async Task DeleteAsync(TaskItem task)
    {
        var sql = @"DELETE FROM ""Tasks"" WHERE Id = @Id AND UserId = @UserId";
        await _db.ExecuteAsync(sql, new { task.Id, task.UserId });
    }
}
