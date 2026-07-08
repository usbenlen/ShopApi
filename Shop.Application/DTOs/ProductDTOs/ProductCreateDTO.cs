using System.ComponentModel.DataAnnotations;

namespace Shop.Application.DTOs.ProductDTOs;

/// <summary>
/// DTO для створення нового продукту
/// </summary>
public class ProductCreateDTO
{
    /// <summary>
    /// Назва продукту
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Ціна продукту
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Опис продукту
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Кількість товару на складі
    /// </summary>
    public int StockQty { get; set; }

    /// <summary>
    /// Чи активний продукт
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Ідентифікатор категорії
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Список URL фотографій продукту
    /// </summary>
    public List<string> Images { get; set; } = [];
}