using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;



using TodoApp.DTOs;
using TodoApp.Services;
using TodoApp.Validators;

namespace TodoApp.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return Ok("Hello admin!!!");
        }
    }
}