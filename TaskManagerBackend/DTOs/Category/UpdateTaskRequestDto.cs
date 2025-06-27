using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.DTOs.Category
{
    public class UpdateTaskRequestDto
    {
        [Required]
        [MaxLength(100)]
        [MinLength(5)]
        public string Name { get; set; }
    }
}
