using AutoMapper;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;

namespace Shop.Application.Services;

public class CategoryService(ICategoryRepository _repository, IMapper _mapper) : ICategoryService
{
    public async Task<int?> CreateCategoryAsync(CategoryCreateDTO dto)
    {
        var category = _mapper.Map<Category>(dto);
        return await _repository.CreateCategoryAsync(category);
    }

    public async Task<IReadOnlyList<CategoryReadDTO>> GetCategoriesAsync()
    {
        var categories = await _repository.GetCategoriesAsync();
        List<CategoryReadDTO>? dtos = null;

        if (categories != null && categories.Count > 0) dtos = _mapper.Map<List<CategoryReadDTO>>(categories);

        return dtos;
    }

    public async Task<CategoryReadDTO?> GetCategoryByIdAsync(int id)
    {
        var category = await _repository.GetCategoryByIdAsync(id);
        if (category == null) return null;

        return _mapper.Map<CategoryReadDTO>(category);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        return await _repository.DeleteCategoryAsync(id);
    }

    public async Task<bool> UpdateCategoryAsync(int id, CategoryUpdateDTO dto)
    {
        var category = await _repository.GetCategoryForUpdateAsync(id);
        if (category == null) return false;

        _mapper.Map(dto, category);

        return await _repository.UpdateCategoryAsync(category);
    }
}
