using System.ComponentModel.DataAnnotations;
using TaskManagerBackend.DTOs.Task;

namespace TaskManagerBackend.DTOs.User
{
    /// <summary>
    /// User data transfer object
    /// </summary>
    public sealed record UserDto
    {
        public string Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Role { get; set; }
        public string FullName => $"{FirstName} {LastName}";

        [EmailAddress]
        public string Email { get; init; }

        public DateTime UserCreated { get; init; }
        public DateTime? UserModified { get; init; }

        public List<TaskStatusCount> TaskStatusCounts { get; set; } = new();


    }
}
