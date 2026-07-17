using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Domain.Models;

[Table("categories")]
public class Category : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("slug")]
    public string Slug { get; set; } = string.Empty;

    // Self-referencing (Підкатегорія)
    [Column("parent_id")]
    public int? ParentId { get; set; } = null;

    [ForeignKey(nameof(ParentId))]
    public Category? Parent { get; set; }

    [MaxLength(100)]
    [Column("description")]
    public string Description { get; set; } = string.Empty; //Опис вже не дуже треба (потім видалити)

    [Column("url")]
    public string ImageURL { get; set; } = string.Empty;
    [Column("is_active")]
    public bool IsActive { get; set; } = false;

    // Navigation properties
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
