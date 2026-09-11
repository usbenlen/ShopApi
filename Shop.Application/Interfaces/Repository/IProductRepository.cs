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
    Task<int?> CreateProductAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отримує список усіх продуктів
    /// </summary>
    /// <returns>Колекція продуктів</returns>
    Task<IReadOnlyList<Product>> GetProductsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Отримує продукт за його ідентифікатором
    /// </summary>
    /// <param name="id">Ідентифікатор продукту</param>
    /// <returns>Продукт або null</returns>
    Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default); // Для читання (AsNoTracking)
    Task<Product?> GetProductForUpdateAsync(int id, CancellationToken cancellationToken = default); // Для оновлення (без AsNoTracking)

    /// <summary>
    /// Видаляє продукт
    /// </summary>
    /// <param name="id">Ідентифікатор продукту</param>
    /// <returns>True, якщо продукт успішно видалено</returns>
    Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Оновлює інформацію про продукт
    /// </summary>
    /// <param name="product">Оновлений продукт</param>
    /// <returns>True, якщо продукт успішно оновлено</returns>
    Task<bool> UpdateProductAsync(CancellationToken cancellationToken = default);
}