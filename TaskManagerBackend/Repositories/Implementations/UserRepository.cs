using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManagerBackend.Data;
using TaskManagerBackend.DTOs.Task;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories.Interfaces;

namespace TaskManagerBackend.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration configuration;
        private readonly TaskDbContext _dbContext;

        public UserRepository(IConfiguration configuration, TaskDbContext dbContext)
        {
            this.configuration = configuration;
            _dbContext = dbContext;
        }
        public string CreateJWTToken(User user, List<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public DateTime? GetTokenExpiration(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(jwtToken))
                return null;

            var token = handler.ReadJwtToken(jwtToken);

            return token.ValidTo;
        }

        public async Task<Dictionary<string, List<TaskStatusCount>>> GetUserTaskStatusCountsAsync(IEnumerable<string> userIds)
        {
            var result = await _dbContext.TaskAssignments
             .Where(ta => userIds.Contains(ta.UserId))
             .Select(ta => new
             {
                 ta.UserId,
                 ta.Task.StatusId
             })
             .GroupBy(x => new { x.UserId, x.StatusId })
             .Select(g => new
             {
                 g.Key.UserId,
                 g.Key.StatusId,
                 Count = g.Count()
             })
             .ToListAsync();


            var count = new Dictionary<string, List<TaskStatusCount>>();

            foreach (var entry in result)
            {
                if (!count.ContainsKey(entry.UserId))
                    count[entry.UserId] = new List<TaskStatusCount>();

                count[entry.UserId].Add(new TaskStatusCount
                {
                    StatusId = entry.StatusId,
                    Count = entry.Count
                });
            }

            return count;

        }

    }
}
