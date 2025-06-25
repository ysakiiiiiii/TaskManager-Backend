using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.Models.Domain
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreatedById { get; set; }
        public int CategoryId { get; set; }
        public int PriorityId { get; set; }
        public int StatusId { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; }
        public DateTime? DueDate { get; set; }

        public ICollection<TaskAssignment> AssignedUsers { get; set; } = new List<TaskAssignment>();
        public ICollection<CheckList> CheckListItems { get; set; } = new List<CheckList>();
        public ICollection<Comment>? Comments { get; set; } = new List<Comment>();
        public ICollection<Attachment>? Attachments { get; set; } = new List<Attachment>();


        //Foreign Keys
        public User CreatedBy { get; set; }
        public Category Category { get; set; }
        public Priority Priority { get; set; }
        public Status Status { get; set; }



    }
}
