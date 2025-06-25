using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.DTOs.Comment
{
    public class CreateCommentDto
    {
        [Required] 
        public int TaskId { get; set; }

        [Required]
        [MaxLength(2000)] 
        public string Content { get; set; }
    }
}
