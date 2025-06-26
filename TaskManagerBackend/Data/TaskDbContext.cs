using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.Models.Domain;
using TaskItem = TaskManagerBackend.Models.Domain.TaskItem;

namespace TaskManagerBackend.Data
{
    public class TaskDbContext : IdentityDbContext<User>
    {
        public TaskDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CheckList> ChecklistItems { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var adminRoleId = "09f53d7c-72bd-4582-a57f-ac35e3c61f2f";
            var userRoleId = "e4fb4f0c-224b-46bb-b479-e22718f6a8fe";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id=adminRoleId,
                    ConcurrencyStamp=adminRoleId,
                    Name="Admin",
                    NormalizedName="Admin".ToUpper()
                },
                new IdentityRole
                {
                    Id=userRoleId,
                    ConcurrencyStamp=userRoleId,
                    Name="User",
                    NormalizedName= "User".ToUpper()
                }

            };

            builder.Entity<IdentityRole>().HasData(roles);


            var priorities = new List<Priority>
            {
                new Priority{Id=1,Name="Low"},
                new Priority{Id=2,Name="Medium"},
                new Priority{Id=3,Name="High"}
            };

            builder.Entity<Priority>().HasData(priorities);


            var statuses = new List<Status>
            {
                new Status{Id=1,Name="To Do"},
                new Status{Id=2,Name="In Progress"},
                new Status{Id=3,Name="Done"},
                new Status{Id=4,Name="On Hold"}
            };

            builder.Entity<Status>().HasData(statuses);

            //TASK RELATIONSHIPS MODEL DEFINITIONS
            builder.Entity<TaskItem>()
                .HasOne(t => t.CreatedBy)
                .WithMany(u => u.CreatedTasks)
                .HasForeignKey(t => t.CreatedById)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.Entity<TaskItem>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tasks)
                .HasForeignKey(t => t.CategoryId);

            builder.Entity<TaskItem>()
                .HasOne(t => t.Priority)
                .WithMany()
                .HasForeignKey(t => t.PriorityId);

            builder.Entity<TaskItem>()
                .HasOne(t => t.Status)
                .WithMany()
                .HasForeignKey(t => t.StatusId);

            //TASK COMMENT RELATIONSHIP DEFINITION
            builder.Entity<Comment>()
                .HasOne(c => c.Task)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TaskId);

            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //TASK ATTACHMENT RELATIONSHIP DEFINITION
            builder.Entity<Attachment>()
                .HasOne(a => a.Task)
                .WithMany(t => t.Attachments)
                .HasForeignKey(a => a.TaskId);

            builder.Entity<Attachment>()
                .HasOne(a => a.UploadedBy)
                .WithMany()
                .HasForeignKey(a => a.UploadedById)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<TaskAssignment>()
                .ToTable("TaskAssignments");

            builder.Entity<TaskAssignment>()
                .HasKey(ta => new { ta.TaskId, ta.UserId });

            builder.Entity<TaskAssignment>()
                .HasOne(ta => ta.Task)
                .WithMany(t => t.AssignedUsers)
                .HasForeignKey(ta => ta.TaskId);

            builder.Entity<TaskAssignment>()
                .HasOne(ta => ta.User)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(ta => ta.UserId);


            builder.Entity<CheckList>()
                .HasOne(cl => cl.Task)
                .WithMany(t => t.CheckListItems)
                .HasForeignKey(cl => cl.TaskId);

        }
    }
}
 