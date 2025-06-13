using System.Data;
using Dapper;

namespace TodoApp.Data;
public static class DbInitializer
{
    public static void Initialize(IDbConnection db)
    {
        var createUsersTable = @"
            CREATE TABLE IF NOT EXISTS ""Users"" (
                Id SERIAL PRIMARY KEY,
                Username VARCHAR(50) NOT NULL,
                Email VARCHAR(100) NOT NULL,
                PasswordHash TEXT NOT NULL,
                Role TEXT NOT NULL DEFAULT 'User'
            );";

        var createTasksTable = @"
            CREATE TABLE IF NOT EXISTS ""Tasks"" (
                Id SERIAL PRIMARY KEY,
                Title TEXT NOT NULL,
                Description TEXT NOT NULL,
                IsCompleted BOOLEAN DEFAULT FALSE,
                UserId INTEGER NOT NULL,
                CONSTRAINT fk_user FOREIGN KEY(UserId) REFERENCES ""Users""(Id) ON DELETE CASCADE
            );";

        db.Execute(createUsersTable);
        db.Execute(createTasksTable);
    }
}
