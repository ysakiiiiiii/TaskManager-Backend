using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.DTOs.User
{
    public sealed record RegisterRequestDto
    {
        [Required(ErrorMessage = "First name is required")]
        [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string FirstName { get; init; }

        [Required(ErrorMessage = "Last name is required")]
        [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        public string LastName { get; init; }

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        [DataType(DataType.EmailAddress)]
        public string Username { get; init; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Password must contain uppercase, lowercase and number")]
        public string Password { get; init; }
    }
}
