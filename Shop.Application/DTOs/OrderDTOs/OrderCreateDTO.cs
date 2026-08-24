namespace Shop.Application.DTOs.OrderDTOs;

public class CreateOrderDTO
{
    public List<OrderProductDTO> Products { get; set; } = [];
}
