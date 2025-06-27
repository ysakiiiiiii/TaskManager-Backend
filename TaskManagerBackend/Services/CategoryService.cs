using AutoMapper;
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
        public async Task<CategoryDto> CreateCategoryAsync(AddCategoryRequestDto categoryRequestDto)
        {
            var category = new Category
            {
                Name = categoryRequestDto.Name
            };

            var createdCategory = await categoryRepository.CreateCategoryAsync(category);

            return mapper.Map<CategoryDto>(createdCategory);
        }
    }
}
