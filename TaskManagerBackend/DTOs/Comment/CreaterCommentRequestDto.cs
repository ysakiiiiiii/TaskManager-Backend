using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.DTOs.Comment
{
    public class CreateCommentRequestDto
    {
        [Required]
        [MinLength(5)]
        [MaxLength(2000)] 
        public string Content { get; set; }
    }
}
