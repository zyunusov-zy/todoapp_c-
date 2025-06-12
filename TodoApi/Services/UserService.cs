using TodoApp.Models;
using TodoApp.DTOs;
using TodoApp.Repositories;
using Microsoft.AspNetCore.Identity;
using TodoApp.Utils;


namespace TodoApp.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;
    private readonly PasswordHasher<User> _passwordHasher;

    private readonly IConfiguration _conf;

    public UserService(IUserRepository repo, IConfiguration conf)
    {
        _repo = repo;
        _passwordHasher = new PasswordHasher<User>();
        _conf = conf;
    }

    public async Task<UserResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existing = await _repo.GetByEmailAsync(dto.Email);
        if (existing != null)
        {
            throw new Exception("Email is already in use!");
        }

        var user = new User
        {
            Email = dto.Email,
            Username = dto.Username
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        await _repo.AddAsync(user);
        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        var user = await _repo.GetByEmailAsync(dto.Email);
        if (user == null)
            throw new Exception("Invalid credentials");
        if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) == PasswordVerificationResult.Failed)
            throw new Exception("Invalid credentials");

        var token = new JwtToken(_conf).GenerateJwtToken(user);
        return token;
    }

}