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
        Task<PaginatedUserResponse> GetPaginatedUsersAsync(bool? isActive, int page, int pageSize);
        Task<ApiResponse> ToggleUserActivationAsync(string userId);
        Task<ApiResponse> GetCurrentUserAsync(ClaimsPrincipal user);

    }
}
