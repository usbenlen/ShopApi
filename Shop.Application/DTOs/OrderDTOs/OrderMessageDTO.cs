using Shop.Domain.Enums;

namespace Shop.Application.DTOs.OrderDTOs;

public class OrderMessageDTO
{
    public Guid UserId { get; set; }
    public OrderStatus Status { get; set; }
    public bool Paid { get; set; }
    public List<OrderMessageProductDTO> Products { get; set; } = [];
    public decimal TotalPrice { get; set; }
}
