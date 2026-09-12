namespace Shop.Application.DTOs.ProductDTOs;

/// <summary>
/// DTO для отримання інформації про продукт
/// </summary>
public class ProductReadDTO
{
    /// <summary>
    /// Ідентифікатор продукту
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Назва продукту
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Ціна товару до знижки
    /// </summary>
    public decimal Price { get; set; }

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
    /// Список URL фотографій продукту
    /// </summary>
    public List<ProductImageReadDTO> Images { get; set; } = [];
}