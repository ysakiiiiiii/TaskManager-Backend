using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.DTOs.User;
using TaskManagerBackend.Helpers;
using TaskManagerBackend.Services;
using TaskManagerBackend.Services.Interfaces;

namespace TaskManagerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto)
        {
            var response = await _userService.RegisterAsync(registerRequestDto);
            return response.Success
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            var response = await _userService.LoginAsync(loginRequestDto);
            return response.Success
                ? Ok(response)
                : Unauthorized(response);
        }

        [HttpPatch("ToggleActivation/{userId}")]
        public async Task<IActionResult> ToggleActivation(string userId)
        {
            var response = await _userService.ToggleUserActivationAsync(userId);
            return response.Success
                ? Ok(response)
                : NotFound(response);
        }
    }
}
