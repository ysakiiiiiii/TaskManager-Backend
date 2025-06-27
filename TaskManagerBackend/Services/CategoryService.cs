using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.DTOs.Category;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories;

namespace TaskManagerBackend.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await categoryRepository.GetAllCategoryAsync();

            return mapper.Map<List<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var categoryDomainModel = await categoryRepository.GetCategoryByIdAsync(id);


            return mapper.Map<CategoryDto>(categoryDomainModel);
        }


        public async Task<CategoryDto> CreateCategoryAsync(AddCategoryRequestDto categoryRequestDto)
        {
            var category = new Category
            {
                Name = categoryRequestDto.Name
            };

            var createdCategory = await categoryRepository.CreateCategoryAsync(category);

            return mapper.Map<CategoryDto>(createdCategory);
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryRequestDto updateCategoryRequestDto)
        {
            var existingCategory = await categoryRepository.GetCategoryByIdAsync(id);

            if (existingCategory == null) return null;

            existingCategory.Name = updateCategoryRequestDto.Name;

            var updatedCategory = await categoryRepository.UpdateCategoryAsync(id, existingCategory);
            return mapper.Map<CategoryDto>(updatedCategory);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var result = await categoryRepository.DeleteCategoryAsync(id);
            return result != null;
        }

    }
}
