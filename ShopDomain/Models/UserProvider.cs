using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Domain.Models;

[Table("user_providers")]
public class UserProvider : BaseEntity
{
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("provider_id")]
    public int ProviderId { get; set; }

    /// <summary>
    /// ID користувача у зовнішньому OAuth provider.
    /// </summary>
    [Required]
    [Column("number_provider")]
    public string NumberProvider { get; set; } = string.Empty;

    // navigation
    public User User { get; set; } = null!;

    public Provider Provider { get; set; } = null!;
}
