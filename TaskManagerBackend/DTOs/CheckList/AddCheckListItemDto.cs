using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace TaskManagerBackend.DTOs.CheckList
{
    public class AddCheckListItemDto
    {
        public string Description { get; set; } = string.Empty;
    }
}
