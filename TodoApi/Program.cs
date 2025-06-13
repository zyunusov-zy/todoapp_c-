using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Validators;
using TodoApp.Repositories;
using TodoApp.Services;
using TodoApp.DTOs;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);

Env.Load();



var connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                       $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                       $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                       $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                       $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Todo API", Version = "v1" });

    // Add JWT Auth to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in this format: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

builder.Services.AddDbContext<TodoAppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegistrationDtoValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginDtoValidation>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();


builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "TodoApp",
            ValidateAudience = true,
            ValidAudience = "TodoAppAudience",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!))
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Migrations auto
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoAppDbContext>();
    db.Database.Migrate();
}

app.UseAuthentication();
app.UseAuthorization();


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

app.MapPost("/login", async (
    LoginDto dto,
    IValidator<LoginDto> validator,
    IUserService userService
) =>
{
    var validationResult = await validator.ValidateAsync(dto);
    if (!validationResult.IsValid)
    {
        var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
        return Results.BadRequest(errors);
    }

    try
    {
        var token = await userService.LoginAsync(dto);
        return Results.Ok(new { token });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

// validate 
app.MapPost("/tasks", [Authorize] async (
    CreateTaskDto dto,
    HttpContext http,
    ITaskService service) =>
{
    var claim = http.User.FindFirst(ClaimTypes.NameIdentifier);

    if (claim is null)
    {
        return Results.Unauthorized();
    }

    var userId = int.Parse(claim.Value);
    var task = await service.CreateTaskAsync(dto, userId);
    return Results.Ok(task);
});


app.MapGet("/tasks", [Authorize] async (
    HttpContext http,
    ITaskService service) =>
{

    
    var claim = http.User.FindFirst(ClaimTypes.NameIdentifier);

    if (claim is null)
    {
        return Results.Unauthorized();
    }

    var userId = int.Parse(claim.Value);
    var tasks = await service.GetAllTasksAsync(userId);
    return Results.Ok(tasks);
});

app.MapGet("/tasks/{id}", [Authorize] async (
    int id,
    HttpContext http,
    ITaskService service) =>
{
    var claim = http.User.FindFirst(ClaimTypes.NameIdentifier);

    if (claim is null)
    {
        return Results.Unauthorized();
    }

    var userId = int.Parse(claim.Value);
    var task = await service.GetTaskByIdAsync(id, userId);
    return task is not null ? Results.Ok(task) : Results.NotFound();
});

// validate
app.MapPut("/tasks/{id}", [Authorize] async (
    int id,
    UpdateTaskDto dto,
    HttpContext http,
    ITaskService service) =>
{
    var claim = http.User.FindFirst(ClaimTypes.NameIdentifier);

    if (claim is null)
    {
        return Results.Unauthorized();
    }

    var userId = int.Parse(claim.Value);
    try
    {
        await service.UpdateTaskAsync(id, dto, userId);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapDelete("/tasks/{id}", [Authorize] async (
    int id,
    HttpContext http,
    ITaskService service) =>
{
    var claim = http.User.FindFirst(ClaimTypes.NameIdentifier);

    if (claim is null)
    {
        return Results.Unauthorized();
    }

    var userId = int.Parse(claim.Value);
    try
    {
        await service.DeleteTaskAsync(id, userId);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});


app.MapGet("/admin", [Authorize(Roles = "Admin")] () => "Hello, Admin!!!");


app.Run();
