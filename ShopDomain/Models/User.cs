using Shop.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Domain.Models;

[Table("users")]
public class User : BaseEntity
{
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [EmailAddress]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("password_hash")]
    public string? PasswordHash { get; set; }

    [Column("role")]
    public UserRole Role { get; set; } = UserRole.User;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("is_email_verified")]
    public bool IsEmailVerified { get; set; } = false;

    //navigation
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<UserAddress> Addresses { get; set; } = new List<UserAddress>();
    public ICollection<UserProvider> UserProviders { get; set; } = new List<UserProvider>();
}