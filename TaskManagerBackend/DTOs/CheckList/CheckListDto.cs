using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.DTOs.CheckList
{
    /// <summary>
    /// Response DTO for checklist item data
    /// </summary>
    public sealed record CheckListDto
    {
        public int Id { get; init; }

        [Required]
        [StringLength(500)]
        public string Description { get; init; } = string.Empty;

        public bool IsCompleted { get; init; }
    }
}
