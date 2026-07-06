using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;

namespace Shop.Application.Services;

public class CategoryService(ICategoryRepository _repository) : ICategoryService
{
    public async Task<int?> CreateCategoryAsync(CategoryCreateDTO dto)
    {
        //TODO: додати AutoMapper
        return await _repository.CreateCategoryAsync(new Category()
        {
            Name = dto.Name,
            Slug = dto.Slug,
            ImageURL = dto.ImageURL,
            Description = dto.Description,
            ParentId = dto.ParentId,
        });
    }

    public async Task<IReadOnlyList<CategoryReadDTO>> GetCategoriesAsync()
    {
        var categories = await _repository.GetCategoriesAsync();

        return categories.Select(c => new CategoryReadDTO
        {
            Id = c.Id,
            Name = c.Name,
            Slug = c.Slug,
            ParentId = c.ParentId,
            Description = c.Description,
            ImageURL = c.ImageURL
        }).ToArray();
    }

    public async Task<CategoryReadDTO?> GetCategoryByIdAsync(int id)
    {
        var category = await _repository.GetCategoryByIdAsync(id);
        if (category == null) return null;

        return new CategoryReadDTO
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            ParentId = category.ParentId,
            Description = category.Description,
            ImageURL = category.ImageURL
        };
    }
}
