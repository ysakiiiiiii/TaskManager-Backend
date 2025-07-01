using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TaskManagerBackend.DTOs.CheckList;

namespace TaskManagerBackend.DTOs.Task
{
    /// <summary>
    /// Data transfer object for creating a new task
    /// </summary>
    public class AddTaskRequestDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid category ID")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid priority ID")]
        public int PriorityId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid status ID")]
        public int StatusId { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format")]
        [FutureDateValidation(ErrorMessage = "Due date must be in the future")]
        public DateTime? DueDate { get; set; }

        [MinLength(1, ErrorMessage = "At least one assigned user is required")]
        public List<string> AssignedUsersId { get; set; } = new List<string>();

        [EnsureValidChecklistItems(ErrorMessage = "Checklist items contain invalid data")]
        public List<AddCheckListItemDto> ChecklistItems { get; set; } = new List<AddCheckListItemDto>();
        

        /// <summary>
        /// Validates that a date is in the future
        /// </summary>
        public class FutureDateValidationAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value is DateTime date)
                {
                    if (date < DateTime.Now)
                    {
                        return new ValidationResult(ErrorMessage);
                    }
                }
                return ValidationResult.Success;
            }
        }

        /// <summary>
        /// Validates that checklist items are valid
        /// </summary>
        public class EnsureValidChecklistItemsAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value is List<AddCheckListItemDto> items)
                {
                    foreach (var item in items)
                    {
                        if (string.IsNullOrWhiteSpace(item.Description))
                        {
                            return new ValidationResult("Checklist item text cannot be empty");
                        }
                    }
                }
                return ValidationResult.Success;
            }
        }
    }
}