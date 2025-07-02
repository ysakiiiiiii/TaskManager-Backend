using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        string CreateJWTToken(User user, List<string> roles);
        DateTime? GetTokenExpiration(string jwtToken);

    }
}
