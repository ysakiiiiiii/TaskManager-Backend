using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.DTOs.SearchFilters;
using TaskManagerBackend.DTOs.User;
using TaskManagerBackend.Helpers;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Services.Interfaces;

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
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
    {
        var response = await _userService.LoginAsync(loginRequestDto);
        if (!response.Success || response.Data is not LoginResponseDto loginData)
            return Unauthorized(response);

        HttpContext.Response.Cookies.Append("jwt", loginData.JwtToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = loginData.TokenExpiration
        });

        return Ok(ApiResponse.SuccessResponse("Login successful"));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("AllUsers")]
    public async Task<IActionResult> GetAllUsers([FromQuery] bool? isActive, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var response = await _userService.GetPaginatedUsersAsync(isActive, page, pageSize);
        return Ok(ApiResponse.SuccessResponse(response));
    } 

    [Authorize]
    [HttpGet("CurrentUser")]
    public async Task<IActionResult> CurrentUser()
    {
        var response = await _userService.GetCurrentUserAsync(User);
        return response.Success ? Ok(response) : Unauthorized(response);
    }

    [HttpPost("Logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return Ok(ApiResponse.SuccessResponse("Logged out successfully."));
    }
}
