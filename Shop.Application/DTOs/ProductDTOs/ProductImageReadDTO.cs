namespace Shop.Application.DTOs.ProductDTOs;

public class ProductImageReadDTO
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}
