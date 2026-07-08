using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Repository;

public interface ICategoryRepository
{
    Task<int?> CreateCategoryAsync(Category category);
    Task<IReadOnlyList<Category>> GetCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<bool> DeleteCategoryAsync(int id);
    Task<bool> UpdateCategoryAsync(Category category);

}
