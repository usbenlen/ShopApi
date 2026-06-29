using System.ComponentModel.DataAnnotations;

namespace Shop.Domain.DTOs;

/// <summary>
/// DTO для створення та оновлення продукту
/// </summary>
public class ProductDTO
{
    [Required]
    /// <summary>
    /// Назва продукту
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Ціна продукту
    /// </summary>
    public decimal Price { get; set; }
}