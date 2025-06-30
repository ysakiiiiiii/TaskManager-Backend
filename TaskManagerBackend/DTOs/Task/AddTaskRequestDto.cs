using System.ComponentModel.DataAnnotations;
using TaskManagerBackend.DTOs.Attachment;
using TaskManagerBackend.DTOs.Category;
using TaskManagerBackend.DTOs.CheckList;
using TaskManagerBackend.DTOs.Comment;
using TaskManagerBackend.DTOs.TaskAssignment;
using TaskManagerBackend.DTOs.User;

namespace TaskManagerBackend.DTOs.Task
{
    public class AddTaskRequestDto
    {

            [Required]
            [MaxLength(100)]
            public string Title { get; set; }

            [MaxLength(1000)]
            public string Description { get; set; }

            [Required]
            public int CategoryId { get; set; }

            [Required]
            public int PriorityId { get; set; }
            
            [Required]
            public int StatusId { get; set; }

            public DateTime? DueDate { get; set; } = null;
            public List<string> AssignedUsersId { get; set; } = new();
            public List<AddCheckListItemDto> ChecklistItems { get; set; } = new();

    }
}
