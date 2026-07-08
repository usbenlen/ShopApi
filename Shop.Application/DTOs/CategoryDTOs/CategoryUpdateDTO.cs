namespace Shop.Application.DTOs.CategoryDTOs;

public class CategoryUpdateDTO
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public int? ParentId { get; set; }
    public string? Description { get; set; }
    public string? ImageURL { get; set; }
}