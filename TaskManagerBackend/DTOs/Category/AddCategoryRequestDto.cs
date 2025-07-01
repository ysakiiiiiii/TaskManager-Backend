using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.DTOs.Category
{
    /// <summary>
    /// Request DTO for creating a new category
    /// </summary>
    public sealed record AddCategoryRequestDto
    {
        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        [MinLength(3, ErrorMessage = "Category name must be at least 3 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-]+$",
            ErrorMessage = "Category name can only contain letters, numbers, spaces, and hyphens")]
        public string Name { get; init; } = string.Empty;
    }

}
