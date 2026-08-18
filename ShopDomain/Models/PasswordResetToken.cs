using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Domain.Models;

[Table("password_reset_tokens")]
public class PasswordResetToken : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("token_hash")]
    public string TokenHash { get; set; } = string.Empty;

    [Required]
    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }

    [Column("used_at")]
    public DateTime? UsedAt { get; set; }

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}