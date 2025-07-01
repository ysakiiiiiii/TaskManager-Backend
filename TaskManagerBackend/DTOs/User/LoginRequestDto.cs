using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.DTOs.User
{
    /// <summary>
    /// Request DTO for user login
    /// </summary>
    public sealed record LoginRequestDto
    {
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        [DataType(DataType.EmailAddress)]
        public string Username { get; init; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; init; }
    }

}
