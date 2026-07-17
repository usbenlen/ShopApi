namespace Shop.Application.DTOs.CategoryDTOs;

public class CategoryUpdateDTO
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public int? ParentId { get; set; }
    public string? Description { get; set; } //Опис вже не дуже треба (потім видалити)
    public string? ImageURL { get; set; }
}