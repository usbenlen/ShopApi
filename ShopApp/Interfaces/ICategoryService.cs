using Shop.Domain.Models;

namespace Shop.Api.Interfaces;

public interface ICategoryService
{
    List<Category> GetAllCategories();
}
