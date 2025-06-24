using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.DTOs;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> userManager;

        public UserController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto)
        {
            var user = new User
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username,
                FirstName = registerRequestDto.FirstName,
                LastName = registerRequestDto.LastName,
            };

            var identityResult = await userManager.CreateAsync(user, registerRequestDto.Password);

            if (!identityResult.Succeeded)
            {
                return BadRequest(new
                {
                    Errors = identityResult.Errors.Select(e => e.Description)
                });
            }

            var roleResult = await userManager.AddToRoleAsync(user, "User");

            if (!roleResult.Succeeded)
            {
                return BadRequest(new
                {
                    Errors = roleResult.Errors.Select(e => e.Description)
                });
            }

            return Ok("Registered Successfully. Please Login");
        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            var user = await userManager.FindByEmailAsync(loginRequestDto.Username);
            if (user != null)
            {
                var isPasswordValid = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
                if (isPasswordValid)
                {
                    return Ok("Login Successful");
                }
                else
                {
                    return Unauthorized("Invalid Password");
                }
            }
            else
            {
                return NotFound("User not found");
            }
        }
    }
}
