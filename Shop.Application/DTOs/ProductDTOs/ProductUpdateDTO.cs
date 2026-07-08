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

    /// <summary>
    /// Список фотографій продукту
    /// </summary>
    public List<string>? Images { get; set; }
    // Щоб бачити які є картинки на сайті у товару, та можливість їх коригувати, видалити або додати ще. Треба створити нові ендпоїнти та реалізацію того що нижче
    // public List<int>? DeleteImages { get; set; } // видалити картинки [1,3] 
}