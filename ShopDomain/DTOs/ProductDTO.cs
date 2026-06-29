using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models;

public class ProductDTO
{
    [Required]
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
}