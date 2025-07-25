﻿using AutoMapper;
using TaskManagerBackend.DTOs.Category;
using TaskManagerBackend.DTOs.SearchFilters;
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

        public async Task<SearchFiltersDto> GetSearchFiltersAsync()
        {
            var statuses = await _categoryRepository.GetAllStatusesWithIdAsync();
            var priorities = await _categoryRepository.GetAllPrioritiesWithIdAsync();
            var categories = await _categoryRepository.GetAllCategoriesWithIdAsync();

            return new SearchFiltersDto
            {
                Statuses = statuses,
                Priorities = priorities,
                Categories = categories
            };
        }

        public async Task ReassignTasksAndDeleteCategoryAsync(int oldCategoryId, int newCategoryId)
        {
            var oldCategory = await _categoryRepository.GetCategoryByIdAsync(oldCategoryId)
                ?? throw new NotFoundException("Old category not found");

            var newCategory = await _categoryRepository.GetCategoryByIdAsync(newCategoryId)
                ?? throw new NotFoundException("New category not found");

            if (oldCategory.Id == newCategory.Id)
                throw new BadRequestException("Old and new categories cannot be the same.");

            await _categoryRepository.ReassignTasksToCategoryAsync(oldCategoryId, newCategoryId);
            await _categoryRepository.DeleteCategoryAsync(oldCategoryId);
        }

    }
}
