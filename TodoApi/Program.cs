using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Validators;
using TodoApp.Repositories;
using TodoApp.Services;
using TodoApp.DTOs;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

var connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                       $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                       $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                       $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                       $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TodoAppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegistrationDtoValidation>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");

app.MapPost("/register", async (
    RegisterDto dto,
    IValidator<RegisterDto> validator,
    IUserService userService) =>
{
    var validationResult = await validator.ValidateAsync(dto);
    if (!validationResult.IsValid)
    {
        var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
        return Results.BadRequest(errors);
    }
    
    try
    {
        var response = await userService.RegisterAsync(dto);
        return Results.Ok(response);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.Run();
