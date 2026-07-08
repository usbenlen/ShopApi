using Microsoft.AspNetCore.Http;
using Shop.Application.DTOs.CategoryDTOs;

namespace Shop.Api.Requests.Categories;

/// <summary>
/// DTO запиту на оновлення категорії із завантаженням картинки
/// </summary>
public class CategoryUpdateRequest : CategoryUpdateDTO
{
    /// <summary>
    /// Нова картинка категорії
    /// </summary>
    public IFormFile? Image { get; set; }
}