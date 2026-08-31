namespace Shop.Application.DTOs.OrderDTOs;

public class OrderProcessingProductDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Count { get; set; }
}
