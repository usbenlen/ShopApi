using Shop.Application.DTOs.CategoryDTOs;

namespace Shop.Api.Requests.Categories;

public class CategoryCreateRequest : CategoryCreateDTO
{
    public IFormFile? Image { get; set; }
}
