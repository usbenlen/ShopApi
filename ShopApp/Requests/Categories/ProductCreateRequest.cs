using Microsoft.AspNetCore.Http;
using Shop.Application.DTOs.ProductDTOs;

namespace Shop.Api.Requests.Products;

/// <summary>
/// DTO запиту на створення продукту із завантаженням фотографій
/// </summary>
public class ProductCreateRequest : ProductCreateDTO
{
    /// <summary>
    /// Фотографії продукту
    /// </summary>
    public List<IFormFile> ImagesFiles { get; set; } = [];
}