using System.ComponentModel.DataAnnotations;
using TaskManagerBackend.DTOs.Attachment;
using TaskManagerBackend.DTOs.CheckList;
using TaskManagerBackend.Helpers;

namespace TaskManagerBackend.DTOs.Task
{
    public class AddTaskRequestDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Category ID")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Priority ID")]
        public int PriorityId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Status ID")]
        public int StatusId { get; set; }

        [FutureDate(ErrorMessage = "Due date must be in the future")]
        public DateTime? DueDate { get; set; }

        public ICollection<string> AssignedUserIds { get; set; } = new List<string>();

    }
}