using System.ComponentModel.DataAnnotations;

namespace Shop.Application.DTOs.ProductDTOs;

/// <summary>
/// DTO для створення та оновлення продукту
/// </summary>
public class ProductDTO
{
    [Required]
    /// <summary>
    /// Назва продукту
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Ціна продукту
    /// </summary>
    public decimal Price { get; set; }
}