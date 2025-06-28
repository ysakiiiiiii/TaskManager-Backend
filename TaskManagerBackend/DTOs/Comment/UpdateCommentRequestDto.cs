using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.DTOs.Comment
{
    public class UpdateCommentRequestDto
    {
        [MinLength(5)]
        [MaxLength(1000)]
        public string Content { get; set; }
    }
}
