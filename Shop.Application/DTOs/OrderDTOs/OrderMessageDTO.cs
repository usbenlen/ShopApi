namespace Shop.Application.DTOs.OrderDTOs;

public class OrderMessageDTO
{
    public Guid OrderRequestId { get; set; }
    public Guid UserId { get; set; }
    public List<OrderMessageProductDTO> Products { get; set; } = [];
    public decimal TotalPrice { get; set; }
}
