using Shop.Domain.Enums;

namespace Shop.Application.DTOs.ProductFeedbackDTOs;

public class ProductFeedbackCreateDTO
{
    public int ProductId { get; set; }
    public ProductFeedbackType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public double? Rating { get; set; }
}
