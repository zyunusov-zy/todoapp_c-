using TodoApp.Models;
using TodoApp.DTOs;
using TodoApp.Repositories;
using Microsoft.AspNetCore.Identity;


namespace TodoApp.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;
    private readonly PasswordHasher<User> _passwordHasher;

    public UserService(IUserRepository repo)
    {
        _repo = repo;
        _passwordHasher = new PasswordHasher<User>();
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

}