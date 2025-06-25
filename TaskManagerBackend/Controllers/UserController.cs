using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.DTOs.User;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories;

namespace TaskManagerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly ITokenRepository tokenRepository;

        public UserController(UserManager<User> userManager, ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
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

            var roleResult = await userManager.AddToRoleAsync(user, "Admin");

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
                    var roles = await userManager.GetRolesAsync(user);
                    
                    if (roles != null)
                    {
                        var jwtToken = tokenRepository.CreateJWTToken(user, roles.ToList());
                        var response = new LoginResponseDto
                        {
                            JwtToken = jwtToken
                        };

                         return Ok(response);
                    }

                }
                else
                {
                    return Unauthorized("Invalid Password");
                }
            }

           return NotFound("User not found");
        }
    }
}
