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
        public string AssignedToId { get; set; }
        public User AssignedTo { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [Required]
        [MaxLength(20)]
        public string Priority { get; set; }  

        [Required]
        [MaxLength(20)]
        public string Status { get; set; }  

        [Required]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime? DateModified { get; set; }

        public DateTime? DueDate { get; set; }

        public ICollection<ChecklistItem> CheckListItems { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Attachment> Attachments { get; set; }
    }
}
