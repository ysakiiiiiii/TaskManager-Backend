using TaskManagerBackend.DTOs.Category;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Services
{
    public interface ICategoryService
    {
        Task<CategoryDto> CreateCategoryAsync(AddCategoryRequestDto categoryRequestDto);
    }
}
