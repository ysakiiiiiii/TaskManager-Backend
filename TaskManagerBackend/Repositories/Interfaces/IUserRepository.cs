using TaskManagerBackend.DTOs.Task;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Repositories.Interfaces
{
    public interface IUserRepository
    {
        string CreateJWTToken(User user, List<string> roles);
        DateTime? GetTokenExpiration(string jwtToken);
        Task<Dictionary<string, UserTaskCounts>> GetUserTaskBreakdownAsync(IEnumerable<string> userIds);
        Task<UserTaskCounts?> GetSingleUserTaskBreakdownAsync(string userId);



    }
}
