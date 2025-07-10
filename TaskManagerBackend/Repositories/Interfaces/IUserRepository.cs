using TaskManagerBackend.DTOs.Task;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Repositories.Interfaces
{
    public interface IUserRepository
    {
        string CreateJWTToken(User user, List<string> roles);
        DateTime? GetTokenExpiration(string jwtToken);
        Task<Dictionary<string, List<TaskStatusCount>>> GetUserTaskStatusCountsAsync(IEnumerable<string> userIds);


    }
}
