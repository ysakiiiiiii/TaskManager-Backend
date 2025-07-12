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

        public async Task<Dictionary<string, UserTaskCounts>> GetUserTaskBreakdownAsync(IEnumerable<string> userIds)
        {
            var query = _dbContext.TaskAssignments
                .Where(ta => userIds.Contains(ta.UserId))
                .Select(ta => new
                {
                    ta.UserId,
                    ta.Task.StatusId,
                    ta.Task.PriorityId,
                    ta.Task.CategoryId

                });

            var data = await query.ToListAsync();

            var result = new Dictionary<string, UserTaskCounts>();

            foreach (var group in data.GroupBy(x => x.UserId))
            {
                var userCounts = new UserTaskCounts();

                userCounts.StatusCounts = group
                    .GroupBy(x => x.StatusId)
                    .Select(g => new TaskStatusCount { Id = g.Key, Count = g.Count() })
                    .ToList();

                userCounts.PriorityCounts = group
                    .GroupBy(x => x.PriorityId)
                    .Select(g => new TaskPriorityCount { Id = g.Key, Count = g.Count() })
                    .ToList();

                userCounts.CategoryCounts = group
                    .GroupBy(x => x.CategoryId)
                    .Select(g => new TaskCategoryCount { Id = g.Key, Count = g.Count() })
                    .ToList();

                result[group.Key] = userCounts;
            }

            return result;
        }

        public async Task<UserTaskCounts?> GetSingleUserTaskBreakdownAsync(string userId)
        {
            var data = await _dbContext.TaskAssignments
                .Where(ta => ta.UserId == userId)
                .Select(ta => new
                {
                    ta.Task.StatusId,
                    ta.Task.PriorityId,
                    ta.Task.CategoryId
                })
                .ToListAsync();

            if (!data.Any()) return null;

            var result = new UserTaskCounts
            {
                StatusCounts = data
                    .GroupBy(x => x.StatusId)
                    .Select(g => new TaskStatusCount { Id = g.Key, Count = g.Count() })
                    .ToList(),

                PriorityCounts = data
                    .GroupBy(x => x.PriorityId)
                    .Select(g => new TaskPriorityCount { Id = g.Key, Count = g.Count() })
                    .ToList(),

                CategoryCounts = data
                    .GroupBy(x => x.CategoryId)
                    .Select(g => new TaskCategoryCount { Id = g.Key, Count = g.Count() })
                    .ToList()
            };

            return result;
        }
    }
}
