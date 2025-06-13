using System.Data;
using Dapper;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnection _db;

    public UserRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var sql = @"SELECT * FROM ""Users"" WHERE email = @Email";
        return await _db.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
    }

    public async Task AddAsync(User user)
    {
        var sql = @"INSERT INTO ""Users"" (Username, Email, PasswordHash) VALUES (@Username, @Email, @PasswordHash) RETURNING Id";
        await _db.ExecuteScalarAsync<int>(sql, user);
    }
}