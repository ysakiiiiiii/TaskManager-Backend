using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TaskManagerBackend.Models.Domain
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateTime UserCreated { get; set; } = DateTime.UtcNow;
        public DateTime? UserModified { get; set; }

        public string FullName => $"{FirstName} {LastName}";

    }
}
