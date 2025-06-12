using TodoApp.Models;
using TodoApp.DTOs;

namespace TodoApp.Services;

public interface IUserService
{
    Task<UserResponseDto> RegisterAsync(RegisterDto dto);
}

