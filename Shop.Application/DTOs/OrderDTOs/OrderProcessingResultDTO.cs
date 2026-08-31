namespace Shop.Application.DTOs.OrderDTOs;

public class OrderProcessingResultDTO
{
    public bool Success { get; set; }
    public bool WaitingForStock { get; set; }
    public bool AlreadyProcessed { get; set; }
    public int? OrderId { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderProcessingProductDTO> Products { get; set; } = [];
    public List<OrderProcessingProductDTO> UnavailableProducts { get; set; } = [];
}