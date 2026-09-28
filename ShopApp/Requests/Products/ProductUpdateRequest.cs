using Shop.Application.DTOs.ProductDTOs;

namespace Shop.Api.Requests.Products;

public class ProductUpdateRequest : ProductUpdateDTO
{
    /// <summary>
    /// Нові фотографії продукту
    /// </summary>
    public List<IFormFile> ImagesFiles { get; set; } = [];
}