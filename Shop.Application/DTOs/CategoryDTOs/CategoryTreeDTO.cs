namespace Shop.Application.DTOs.CategoryDTOs;

public class CategoryTreeDTO
{
    public int Id { get; set;  }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public ICollection<CategoryTreeDTO> SubCategories { get; set; } = [];

}
