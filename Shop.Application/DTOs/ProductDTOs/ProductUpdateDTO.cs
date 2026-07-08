namespace Shop.Application.DTOs.ProductDTOs;

/// <summary>
/// DTO для оновлення інформації про продукт
/// </summary>
public class ProductUpdateDTO
{
    /// <summary>
    /// Назва продукту
    /// </summary>
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
    public bool IsActive { get; set; }

    /// <summary>
    /// Ідентифікатор категорії
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Список фотографій продукту
    /// </summary>
    public List<string> Images { get; set; } = [];
}