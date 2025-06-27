using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.DTOs.Category
{
    public class AddCategoryRequestDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }

}
