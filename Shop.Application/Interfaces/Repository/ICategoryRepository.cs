using Shop.Application.DTOs.CategoryDTOs;
using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Repository;

public interface ICategoryRepository
{
    Task<int?> CreateCategoryAsync(Category category, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<Category?> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Category?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default); // Для читання (AsNoTracking)
    Task<Category?> GetCategoryForUpdateAsync(int id, CancellationToken cancellationToken = default); // Для оновлення (без AsNoTracking)
    Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> UpdateCategoryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetParentCategoriesAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetChildCategoriesAsync(int id, CancellationToken cancellationToken = default);
    Task<Category?> GetCategoryTreeAsync(int id, CancellationToken cancellationToken = default);

}
