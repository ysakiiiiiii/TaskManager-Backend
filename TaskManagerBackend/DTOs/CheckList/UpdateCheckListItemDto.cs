using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.DTOs.CheckList
{
    /// <summary>
    /// Request DTO for updating a checklist item
    /// </summary>
    public sealed record UpdateCheckListRequestDto
    {
        [Required]
        public List<UpdateChecklistItemDto> Items { get; init; } = new();
    }

    public sealed record UpdateChecklistItemDto
    {
        [Required(ErrorMessage = "Checklist id is required")]
        public int Id { get; init; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, MinimumLength = 3,ErrorMessage = "Description must be between 3 and 500 characters")]
        [DataType(DataType.MultilineText)]
        public string Description { get; init; } = string.Empty;
    }
}
