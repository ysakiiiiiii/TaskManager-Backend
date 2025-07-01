using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.DTOs.CheckList
{
/// <summary>
    /// Request DTO for updating a checklist item
    /// </summary>
    public sealed record UpdateCheckListItemDto
    {
        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, MinimumLength = 3,ErrorMessage = "Description must be between 3 and 500 characters")]
        [DataType(DataType.MultilineText)]
        public string Description { get; init; } = string.Empty;
    }
}
