using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Domain.Models;

[Table("providers")]
public class Provider
{
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    // navigation
    public ICollection<UserProvider> UserProviders { get; set; } = new List<UserProvider>();
}
