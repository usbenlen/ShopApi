namespace Shop.Domain.Models;

/// <summary>
/// Модель продукту
/// </summary>
public class Product
{
    /// <summary>
    /// Ідентифікатор продукту
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Назва продукту
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Ціна продукту
    /// </summary>
    public decimal Price { get; set; }
}
