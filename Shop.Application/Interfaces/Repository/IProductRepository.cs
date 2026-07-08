using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Repository;

/// <summary>
/// Репозиторій для роботи з продуктами
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Створює новий продукт
    /// </summary>
    /// <param name="product">Продукт для створення</param>
    /// <returns>Ідентифікатор створеного продукту</returns>
    Task<int?> CreateProductAsync(Product product);

    /// <summary>
    /// Отримує список усіх продуктів
    /// </summary>
    /// <returns>Колекція продуктів</returns>
    Task<IReadOnlyList<Product>> GetProductsAsync();

    /// <summary>
    /// Отримує продукт за його ідентифікатором
    /// </summary>
    /// <param name="id">Ідентифікатор продукту</param>
    /// <returns>Продукт або null</returns>
    Task<Product?> GetProductByIdAsync(int id);

    /// <summary>
    /// Видаляє продукт
    /// </summary>
    /// <param name="id">Ідентифікатор продукту</param>
    /// <returns>True, якщо продукт успішно видалено</returns>
    Task<bool> DeleteProductAsync(int id);

    /// <summary>
    /// Оновлює інформацію про продукт
    /// </summary>
    /// <param name="product">Оновлений продукт</param>
    /// <returns>True, якщо продукт успішно оновлено</returns>
    Task<bool> UpdateProductAsync(Product product);
}