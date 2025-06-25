using TaskManagerBackend.DTOs.Attachment;
using TaskManagerBackend.DTOs.Category;
using TaskManagerBackend.DTOs.CheckList;
using TaskManagerBackend.DTOs.Comment;
using TaskManagerBackend.DTOs.Priority;
using TaskManagerBackend.DTOs.User;
using TaskManagerBackend.DTOs.Status;
using TaskManagerBackend.DTOs.TaskAssignment;


namespace TaskManagerBackend.DTOs.Task
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreatedById { get; set; }
        public int CategoryId { get; set; }
        public int PriorityId { get; set; }
        public int StatusId { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; } 
        public DateTime? DueDate { get; set; } 

        public List<TaskAssignmentDto> AssignedUsers { get; set; } = new();
        public List<CheckListDto> ChecklistItems { get; set; } = new();
        public List<CommentDto>? Comments { get; set; } = new();
        public List<AttachmentDto>? Attachments { get; set; } = new();
    }
}
