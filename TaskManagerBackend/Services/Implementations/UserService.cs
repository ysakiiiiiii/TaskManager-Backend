using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagerBackend.DTOs.SearchFilters;
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
        private readonly IUserRepository _tokenRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;

        public UserService(
            UserManager<User> userManager,
            IUserRepository tokenRepository,
            ILogger<UserService> logger, 
            IMapper mapper
            )
        {
            _userManager = userManager;
            _tokenRepository = tokenRepository;
            _logger = logger;
            _mapper = mapper;
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

        public async Task<ApiResponse> GetCurrentUserAsync(ClaimsPrincipal userPrincipal)
        {
            try
            {
                var userId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return ApiResponse.ErrorResponse("Unauthorized");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return ApiResponse.ErrorResponse("User not found");

                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "User";

                var dto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = role
                };

                var breakdown = await _tokenRepository.GetSingleUserTaskBreakdownAsync(user.Id);
                if (breakdown != null)
                {
                    dto.TaskStatusCounts = breakdown.StatusCounts;
                    dto.TaskPriorityCounts = breakdown.PriorityCounts;
                    dto.TaskCategoryCounts = breakdown.CategoryCounts;
                }

                return ApiResponse.SuccessResponse(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return ApiResponse.ErrorResponse("An error occurred while retrieving user info");
            }
        }



        public async Task<PaginatedUserResponse> GetPaginatedUsersAsync(ClaimsPrincipal userPrincipal, bool? isActive, int page, int pageSize)
        {
            var requestingUserId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            var requestingUser = await _userManager.FindByIdAsync(requestingUserId);
            var roles = await _userManager.GetRolesAsync(requestingUser);
            var isAdmin = roles.Contains("Admin");

            var usersQuery = _userManager.Users.AsQueryable();

            if (isActive.HasValue)
                usersQuery = usersQuery.Where(u => u.IsActive == isActive.Value);

            if (!isAdmin)
            {
                var adminUserIds = (await _userManager.GetUsersInRoleAsync("Admin")).Select(u => u.Id);
                usersQuery = usersQuery.Where(u => !adminUserIds.Contains(u.Id));
            }

            var totalCount = await usersQuery.CountAsync();

            if (pageSize == 0)
                pageSize = totalCount == 0 ? 1 : totalCount;

            var pagedUsers = await usersQuery
                .OrderBy(u => u.LastName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userDtos = _mapper.Map<List<UserDto>>(pagedUsers);

            var userIds = pagedUsers.Select(u => u.Id);
            var taskBreakdown = await _tokenRepository.GetUserTaskBreakdownAsync(userIds);

            foreach (var userDto in userDtos)
            {
                if (taskBreakdown.TryGetValue(userDto.Id, out var breakdown))
                {
                    userDto.TaskStatusCounts = breakdown.StatusCounts;
                    userDto.TaskPriorityCounts = breakdown.PriorityCounts;
                    userDto.TaskCategoryCounts = breakdown.CategoryCounts;
                }
            }

            return new PaginatedUserResponse
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Items = userDtos
            };
        }



        public async Task<ApiResponse> GetUserStatsAsync(ClaimsPrincipal userPrincipal)
        {
            try
            {
                var userId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return ApiResponse.ErrorResponse("Unauthorized");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return ApiResponse.ErrorResponse("User not found");

                var roles = await _userManager.GetRolesAsync(user);
                var isAdmin = roles.Contains("Admin");

                if (isAdmin)
                {
                    var allUsers = await _userManager.Users.ToListAsync();
                    var userDtos = _mapper.Map<List<UserDto>>(allUsers);

                    var userIds = allUsers.Select(u => u.Id);
                    var taskBreakdowns = await _tokenRepository.GetUserTaskBreakdownAsync(userIds);

                    foreach (var dto in userDtos)
                    {
                        if (taskBreakdowns.TryGetValue(dto.Id, out var breakdown))
                        {
                            dto.TaskStatusCounts = breakdown.StatusCounts;
                            dto.TaskPriorityCounts = breakdown.PriorityCounts;
                            dto.TaskCategoryCounts = breakdown.CategoryCounts;
                        }
                    }

                    return ApiResponse.SuccessResponse(userDtos);
                }
                else
                {
                    var dto = _mapper.Map<UserDto>(user);
                    var breakdown = await _tokenRepository.GetSingleUserTaskBreakdownAsync(user.Id);

                    if (breakdown != null)
                    {
                        dto.TaskStatusCounts = breakdown.StatusCounts;
                        dto.TaskPriorityCounts = breakdown.PriorityCounts;
                        dto.TaskCategoryCounts = breakdown.CategoryCounts;
                    }

                    return ApiResponse.SuccessResponse(new List<UserDto> { dto });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user stats");
                return ApiResponse.ErrorResponse("An error occurred while fetching user stats.");
            }
        }

    }
}
