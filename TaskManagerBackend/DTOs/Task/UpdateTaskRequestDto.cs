using System.ComponentModel.DataAnnotations;
using TaskManagerBackend.DTOs.CheckList;

namespace TaskManagerBackend.DTOs.Task
{
    public class UpdateTaskRequestDto
    {
        [MaxLength(100)]
        public string? Title { get; set; }
        [MaxLength(1000)]
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public int? PriorityId { get; set; }
        public int? StatusId { get; set; }
        public DateTime? DueDate { get; set; }
        public List<AddCheckListItemDto>? ChecklistItems { get; set; }
        public List<string>? AssignedUserIds { get; set; }
    }
}

