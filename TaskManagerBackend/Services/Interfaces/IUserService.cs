using System.Security.Claims;
using TaskManagerBackend.DTOs.SearchFilters;
using TaskManagerBackend.DTOs.User;
using TaskManagerBackend.Helpers;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse> RegisterAsync(RegisterRequestDto registerRequestDto);
        Task<ApiResponse> LoginAsync(LoginRequestDto loginRequestDto);
        Task<PaginatedUserResponse> GetPaginatedUsersAsync(ClaimsPrincipal userPrincipal, bool? isActive, int page, int pageSize, string? search = null);
        Task<ApiResponse> ToggleUserStatusAsync(string userId);
        Task<ApiResponse> GetCurrentUserAsync(ClaimsPrincipal user);
        Task<ApiResponse> GetUserStatsAsync(ClaimsPrincipal userPrincipal);



    }
}
