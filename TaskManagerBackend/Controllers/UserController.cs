using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.DTOs.User;
using TaskManagerBackend.Helpers;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories;

namespace TaskManagerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenRepository _tokenRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserManager<User> userManager,
            ITokenRepository tokenRepository,
            ILogger<UserController> logger)
        {
            _userManager = userManager;
            _tokenRepository = tokenRepository;
            _logger = logger;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.ErrorResponse("Invalid data", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)));
                }

                var user = new User
                {
                    UserName = registerRequestDto.Username,
                    Email = registerRequestDto.Username,
                    FirstName = registerRequestDto.FirstName,
                    LastName = registerRequestDto.LastName,
                };

                var identityResult = await _userManager.CreateAsync(user, registerRequestDto.Password);

                if (!identityResult.Succeeded)
                {
                    return BadRequest(ApiResponse.ErrorResponse("Registration failed",
                        identityResult.Errors.Select(e => e.Description)));
                }

                var roleResult = await _userManager.AddToRoleAsync(user, "User");

                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    return BadRequest(ApiResponse.ErrorResponse("Role assignment failed",
                        roleResult.Errors.Select(e => e.Description)));
                }

                return Ok(ApiResponse.SuccessResponse(null, "Registered successfully. Please login."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(500, ApiResponse.ErrorResponse("An error occurred during registration"));
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.ErrorResponse("Invalid data", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)));
                }

                var user = await _userManager.FindByEmailAsync(loginRequestDto.Username);
                if (user == null)
                {
                    return NotFound(ApiResponse.ErrorResponse("Invalid username or password"));
                }

                var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
                if (!isPasswordValid)
                {
                    return Unauthorized(ApiResponse.ErrorResponse("Invalid username or password"));
                }

                var roles = await _userManager.GetRolesAsync(user);

                if (roles == null || !roles.Any())
                {
                    return StatusCode(500, ApiResponse.ErrorResponse("User has no roles assigned"));
                }

                var jwtToken = _tokenRepository.CreateJWTToken(user, roles.ToList());
                var response = new LoginResponseDto
                {
                    JwtToken = jwtToken
                };

                return Ok(ApiResponse.SuccessResponse(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, ApiResponse.ErrorResponse("An error occurred during login"));
            }
        }
    }
}
