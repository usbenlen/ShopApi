namespace Shop.Application.DTOs.CategoryDTOs;

public class CategoryReadDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int? ParentId { get; set; } = null;
    public string Description { get; set; } = string.Empty; //Опис вже не дуже треба (потім видалити)
    public string ImageURL { get; set; } = string.Empty;
    public ICollection<int>? Products { get; set; }
}
