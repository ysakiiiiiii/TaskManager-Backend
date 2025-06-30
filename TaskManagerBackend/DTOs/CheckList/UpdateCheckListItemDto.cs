using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.DTOs.CheckList
{
    public class UpdateCheckListItemDto
    {
        [Required]
        public string Description { get; set; } = string.Empty;

    }
}
