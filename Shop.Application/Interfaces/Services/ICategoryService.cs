using Shop.Application.DTOs.CategoryDTOs;
using Shop.Domain.Models;


namespace Shop.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<int?> CreateCategoryAsync(CategoryCreateDTO dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CategoryReadDTO>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<CategoryReadDTO?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> UpdateCategoryAsync(int id, CategoryUpdateDTO dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CategoryReadDTO>> GetParentCategoriesAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CategoryReadDTO>> GetChildCategoriesAsync(int id, CancellationToken cancellationToken = default);
    Task<CategoryTreeDTO?> GetCategoryTreeAsync(int id, CancellationToken cancellationToken = default);
}
