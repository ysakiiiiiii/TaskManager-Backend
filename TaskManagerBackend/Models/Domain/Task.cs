using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.Models.Domain
{
    public class Task
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public string CreatedById { get; set; }
        public User CreatedBy { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [Required]
        public int PriorityId { get; set; }
        public Priority Priority{ get; set; }

        [Required]
        public int StatusId { get; set; }
        public Status Status { get; set; }

        [Required]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime? DateModified { get; set; }

        public DateTime? DueDate { get; set; }

        public ICollection<TaskAssignment> AssignedUsers { get; set; } = new List<TaskAssignment>();
        public ICollection<ChecklistItem> CheckListItems { get; set; } = new List<ChecklistItem>();
        public ICollection<Comment>? Comments { get; set; } = new List<Comment>();
        public ICollection<Attachment>? Attachments { get; set; } = new List<Attachment>();
    }
}
