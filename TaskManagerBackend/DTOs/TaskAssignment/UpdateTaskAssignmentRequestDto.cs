using System.ComponentModel.DataAnnotations;
using TaskManagerBackend.Helpers;

namespace TaskManagerBackend.DTOs.TaskAssignment
{
    public class UpdateTaskAssignmentRequestDto
    {
        
        [Required(ErrorMessage = "Assigned user ID is required")]
        [ValidGuid(ErrorMessage = "Assigned user ID must be a valid GUID.")]
        public string UserId { get; set; }
    }
}

