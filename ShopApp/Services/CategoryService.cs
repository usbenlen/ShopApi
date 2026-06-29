using Shop.Api.Interfaces;
using Shop.Domain.Models;

namespace Shop.Api.Services;

public class CategoryService : ICategoryService
{
    private List<Category> _categories = new();

    public List<Category> GetAllCategories()
    {
        _categories.Add(new Category()
        {
            Id = 1,
            Title = "Electronics",
            Description = "Everything that needs power to function",
            Image = "path to image",
            CreatedAt = DateTime.Now,
            Visible = true,
        });
        _categories.Add(new Category()
        {
            Id = 2,
            Title = "Food",
            Description = "Everything that is edible",
            Image = "path to image",
            CreatedAt = DateTime.Now,
            Visible = true,
        });
        return _categories;
    }
}
