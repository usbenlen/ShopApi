using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Domain.Models;

/// <summary>
/// Модель продукту
/// </summary>
[Table("products")]
public class Product : BaseEntity
{
    /// <summary>
    /// Ідентифікатор продукту
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Назва продукту
    /// </summary>
    [Required]
    [MaxLength(200)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Опис продукту
    /// </summary>
    [Column("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Ціна продукту
    /// </summary>
    [Required]
    [Column("price", TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    /// <summary>
    /// Кількість продукту на складі
    /// </summary>
    [Column("stock_qty")]
    public int StockQty { get; set; } = 0;

    /// <summary>
    /// Чи активний продукт
    /// </summary>
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    // FK до категорії    [Required]
    /// <summary>
    /// До якої категорії продукт належить
    /// </summary>
    [Column("category_id")]
    public int CategoryId { get; set; }

    /// <summary>
    /// Категорія
    /// </summary>
    [ForeignKey(nameof(CategoryId))]
    public Category Category { get; set; } = null!;

    // Navigation properties
    /// <summary>
    /// Фотографії продукту
    /// </summary>
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
}