using Microsoft.AspNetCore.Http;
using Shop.Application.DTOs.ProductDTOs;

namespace Shop.Api.Requests.Products;

/// <summary>
/// DTO запиту на оновлення продукту із завантаженням фотографій
/// </summary>
public class ProductUpdateRequest : ProductUpdateDTO
{
    /// <summary>
    /// Нові фотографії продукту
    /// </summary>
    public List<IFormFile>? ImagesFiles { get; set; }
}