using Shop.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Domain.Models;

[Table("orders")]
public class Order : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("order_request_id")]
    public Guid OrderRequestId { get; set; }

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [Required]
    [Column("status")]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [Required]
    [Column("paid")]
    public bool Paid { get; set; } = false;

    // Navigation property
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
