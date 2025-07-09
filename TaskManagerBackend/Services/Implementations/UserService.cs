using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagerBackend.DTOs.User;
using TaskManagerBackend.Helpers;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories.Interfaces;
using TaskManagerBackend.Services.Interfaces;

namespace TaskManagerBackend.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenRepository _tokenRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(
            UserManager<User> userManager,
            ITokenRepository tokenRepository,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _tokenRepository = tokenRepository;
            _logger = logger;
        }

        public async Task<ApiResponse> RegisterAsync(RegisterRequestDto registerRequestDto)
        {
            try
            {
                var userCheck = await _userManager.FindByEmailAsync(registerRequestDto.Username);
                if (userCheck != null)
                { 
                    return ApiResponse.ErrorResponse("Account already exist. Please login");
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
                    return ApiResponse.ErrorResponse("Registration failed", identityResult.Errors.Select(e => e.Description));
                }

                var roleResult = await _userManager.AddToRoleAsync(user, "User");

                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    return ApiResponse.ErrorResponse("Role assignment failed", roleResult.Errors.Select(e => e.Description));
                }

                return ApiResponse.SuccessResponse(null, "Registered successfully. Please login.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return ApiResponse.ErrorResponse("An error occurred during registration");
            }
        }

        public async Task<ApiResponse> LoginAsync(LoginRequestDto loginRequestDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginRequestDto.Username);
                if (user == null || !user.IsActive)
                {
                    return ApiResponse.ErrorResponse("Account is deactivated or not found");
                }

                var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
                if (!isPasswordValid)
                {
                    return ApiResponse.ErrorResponse("Invalid username or password");
                }

                var roles = await _userManager.GetRolesAsync(user);
                if (roles == null || !roles.Any())
                {
                    return ApiResponse.ErrorResponse("User has no roles assigned");
                }

                var jwtToken = _tokenRepository.CreateJWTToken(user, roles.ToList());
                var expiration = _tokenRepository.GetTokenExpiration(jwtToken);
                var response = new LoginResponseDto
                {
                    JwtToken = jwtToken,
                    TokenExpiration = expiration
                };

                return ApiResponse.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return ApiResponse.ErrorResponse("An error occurred during login");
            }
        }

        public async Task<ApiResponse> ToggleUserActivationAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse.ErrorResponse("User not found");
                }

                user.IsActive = !user.IsActive;
                user.UserModified = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return ApiResponse.ErrorResponse("Failed to update user activation", result.Errors.Select(e => e.Description));
                }

                var status = user.IsActive ? "activated" : "deactivated";
                return ApiResponse.SuccessResponse(null, $"User has been {status} successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling user activation");
                return ApiResponse.ErrorResponse("An error occurred while toggling user activation");
            }
        }
    }
}
