using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.DTOs.Comment
{
    /// <summary>
    /// Request DTO for updating an existing comment
    /// </summary>
    public sealed record UpdateCommentRequestDto
    {
        [Required(ErrorMessage = "Comment content is required")]
        [MinLength(5, ErrorMessage = "Comment must be at least 5 characters")]
        [MaxLength(2000, ErrorMessage = "Comment cannot exceed 2000 characters")]
        [DataType(DataType.MultilineText)]
        public string Content { get; init; } = string.Empty;
    }
}
