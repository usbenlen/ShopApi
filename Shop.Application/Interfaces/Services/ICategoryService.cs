using Shop.Application.DTOs.CategoryDTOs;
using Shop.Domain.Models;


namespace Shop.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<int?> CreateCategoryAsync(CategoryCreateDTO dto);
    Task<IReadOnlyList<CategoryReadDTO>> GetCategoriesAsync();
    Task<CategoryReadDTO?> GetCategoryByIdAsync(int id);
    Task<bool> DeleteCategoryAsync(int id);
    Task<bool> UpdateCategoryAsync(int id, CategoryUpdateDTO dto);
}
