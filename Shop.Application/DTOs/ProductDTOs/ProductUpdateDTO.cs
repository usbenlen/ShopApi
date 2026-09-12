namespace Shop.Application.DTOs.ProductDTOs;

/// <summary>
/// DTO для оновлення інформації про продукт
/// </summary>
public class ProductUpdateDTO
{
    /// <summary>
    /// Назва продукту
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Ціна продукту
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// Ціна продукту
    /// </summary>
    public decimal? OldPrice { get; set; }

    /// <summary>
    /// Чи має товар активну знижку
    /// </summary>
    public bool IsDiscounted { get; set; }

    /// <summary>
    /// Опис продукту
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Кількість товару на складі
    /// </summary>
    public int? StockQty { get; set; }

    /// <summary>
    /// Чи активний продукт
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Ідентифікатор категорії
    /// </summary>
    public int? CategoryId { get; set; }
}