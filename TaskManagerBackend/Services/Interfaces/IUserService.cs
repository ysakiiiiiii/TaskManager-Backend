using TaskManagerBackend.DTOs.User;
using TaskManagerBackend.Helpers;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse> RegisterAsync(RegisterRequestDto registerRequestDto);
        Task<ApiResponse> LoginAsync(LoginRequestDto loginRequestDto);
        Task<ApiResponse> ToggleUserActivationAsync(string userId);
    }
}
