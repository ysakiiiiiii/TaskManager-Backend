using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.DTOs.Comment
{
/// <summary>
    /// Response DTO for comment data
    /// </summary>
    public sealed record CommentDto
    {
        public int Id { get; init; }
        public int TaskId { get; init; }  
        
        [Required]
        public string UserId { get; init; } = string.Empty;
        
        public string UserName { get; init; } = string.Empty;
        public string UserAvatar { get; init; } = string.Empty;
        
        [Required]
        [MaxLength(2000)]
        public string Content { get; init; } = string.Empty;
        
        public DateTime DateCreated { get; init; }
        public DateTime? DateUpdated { get; init; }
        
        public bool IsEdited => DateUpdated.HasValue;
    }
}
