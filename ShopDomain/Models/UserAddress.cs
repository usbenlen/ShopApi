using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Domain.Models;

[Table("user_addresses")]
public class UserAddress : BaseEntity
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("country")]
    public string Country { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("city")]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Column("street")]
    public string Street { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("building")]
    public string Building { get; set; } = string.Empty;

    [MaxLength(20)]
    [Column("apartment")]
    public string? Apartment { get; set; }

    [MaxLength(20)]
    [Column("postal_code")]
    public string? PostalCode { get; set; }

    // navigation
    public User User { get; set; } = null!;
}
