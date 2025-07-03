using AutoMapper;
using TaskManagerBackend.DTOs.Category;
using TaskManagerBackend.Exceptions;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories.Interfaces;
using TaskManagerBackend.Services.Interfaces;

namespace TaskManagerBackend.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoryAsync();

            if (categories == null || !categories.Any()) 
                throw new NotFoundException("No categories found");
     
            return _mapper.Map<List<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id) ?? throw new NotFoundException($"Category with ID {id} not found");

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> CreateCategoryAsync(AddCategoryRequestDto dto)
        {
            var category = _mapper.Map<Category>(dto);
            var created = await _categoryRepository.CreateCategoryAsync(category);
            return _mapper.Map<CategoryDto>(created);
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryRequestDto dto)
        {
            var existing = await _categoryRepository.GetCategoryByIdAsync(id)
                ?? throw new NotFoundException("Category not found");

            _mapper.Map(dto, existing);
            var updated = await _categoryRepository.UpdateCategoryAsync(id, existing);
            return _mapper.Map<CategoryDto>(updated);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var deleted = await _categoryRepository.DeleteCategoryAsync(id);
            if (deleted == null)
                throw new NotFoundException("Category not found or already deleted");
        }
    }
}
