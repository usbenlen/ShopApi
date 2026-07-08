using Shop.Application.DTOs.ProductDTOs;

namespace Shop.Application.Interfaces.Services;

/// <summary>
/// Сервіс для роботи з продуктами
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Створює новий продукт
    /// </summary>
    /// <param name="dto">Дані для створення продукту</param>
    /// <returns>Ідентифікатор створеного продукту</returns>
    Task<int?> CreateProductAsync(ProductCreateDTO dto);


    /// <summary>
    /// Отримує список усіх продуктів
    /// </summary>
    /// <returns>Список продуктів</returns>
    Task<IReadOnlyList<ProductReadDTO>> GetProductsAsync();


    /// <summary>
    /// Отримує продукт за ідентифікатором
    /// </summary>
    /// <param name="id">Ідентифікатор продукту</param>
    /// <returns>Продукт або null</returns>
    Task<ProductReadDTO?> GetProductByIdAsync(int id);


    /// <summary>
    /// Видаляє продукт за ідентифікатором
    /// </summary>
    /// <param name="id">Ідентифікатор продукту</param>
    /// <returns>True, якщо продукт видалено</returns>
    Task<bool> DeleteProductAsync(int id);


    /// <summary>
    /// Оновлює інформацію про продукт
    /// </summary>
    /// <param name="id">Ідентифікатор продукту</param>
    /// <param name="dto">Нові дані продукту</param>
    /// <returns>True, якщо продукт оновлено</returns>
    Task<bool> UpdateProductAsync(int id, ProductUpdateDTO dto);
}