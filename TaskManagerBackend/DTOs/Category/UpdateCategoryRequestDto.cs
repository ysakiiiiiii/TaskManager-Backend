using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.DTOs.Category
{
    public class UpdateCategoryRequestDto
    {
        [Required]
        [MaxLength(100)]
        [MinLength(5)]
        public string Name { get; set; }
    }
}
