using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace TaskManagerBackend.DTOs.CheckList
{
    /// <summary>
    /// Request DTO for adding a new checklist item
    /// </summary>

    public sealed record AddCheckListItemDto
    {
        [Required]
        public List<AddItemDto> Items { get; init; } = new();
    }
    public sealed record AddItemDto
    {
        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 500 characters")]
        [DataType(DataType.MultilineText)]
        public string Description { get; init; } = string.Empty;
    }
}
