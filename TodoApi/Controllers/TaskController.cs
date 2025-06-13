using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

using TodoApp.DTOs;
using TodoApp.Services;
using TodoApp.Validators;

namespace TodoApp.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class TaskController : ControllerBase
    {

        private readonly ITaskService _taskService;
        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost("tasks")]
        [Authorize]
        public async Task<IActionResult> AddTask([FromBody] CreateTaskDto dto)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim is null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(claim.Value);

            try
            {
                var task = await _taskService.CreateTaskAsync(dto, userId);
                return Ok(task);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpGet("tasks")]
        [Authorize]
        public async Task<IActionResult> GetAllTasks()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim is null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(claim.Value);
            var tasks = await _taskService.GetAllTasksAsync(userId);
            return Ok(tasks);
        }

        [HttpGet("tasks/{id}")]
        [Authorize]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim is null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(claim.Value);
            var task = await _taskService.GetTaskByIdAsync(id, userId);
            return task is not null ? Ok(task) : NotFound();
        }

        [HttpPut("tasks/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTask(int id, UpdateTaskDto dto)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim is null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(claim.Value);
            try
            {
                await _taskService.UpdateTaskAsync(id, dto, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpDelete("tasks/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim is null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(claim.Value);
            try
            {
                await _taskService.DeleteTaskAsync(id, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}