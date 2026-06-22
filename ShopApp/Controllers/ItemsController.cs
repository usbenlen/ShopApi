using Microsoft.AspNetCore.Mvc;
using ShopDomain.Models;

namespace ShopApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    //[HttpGet]
    //public IResult GetItems()
    //{
    //    return Results.Ok(new[] { "Item 1", "Item 2", "І всі Items" });
    //}

    //[HttpGet("{id}")]
    //public IResult GetItemById([FromRoute] int id) 
    //{
    //    return Results.Ok($"Item {id}");
    //}

    // dz

    private static readonly List<Item> Items =
    [
        new Item { Id = 1, Name = "Pen" },
        new Item { Id = 2, Name = "Notebook" },
        new Item { Id = 3, Name = "Tablet" }
    ];

    // GET /api/items
    [HttpGet]
    public IActionResult GetItems()
    {
        return Ok(Items);
    }

    // GET /api/items/1
    [HttpGet("{id}")]
    public IActionResult GetItemById(int id)
    {
        var item = Items.FirstOrDefault(x => x.Id == id);
        if (item == null) return NotFound("Item not found");

        return Ok(item);
    }

    // GET /api/items/search?name=pen
    [HttpGet("search")]
    public IActionResult Search([FromQuery] string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return BadRequest("Query parameter `name` is required");
        var result = Items.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();

        return Ok(result);
    }

    // POST /api/items
    [HttpPost]
    public IActionResult Create(Item item)
    {
        if (string.IsNullOrWhiteSpace(item.Name)) return BadRequest("Name is required");
        if (Items.Any(x => x.Name.ToLower().Equals(item.Name.ToLower()))) return BadRequest("Item with this name already exists");

        var newId = Items.Any() ? Items.Max(x => x.Id) + 1 : 1;
        var newItem = new Item
        {
            Id = newId,
            Name = item.Name
        };

        Items.Add(newItem);

        return Created($"/api/items/{newItem.Id}", newItem);
    }

    // PUT /api/items/4
    [HttpPut("{id}")]
    public IActionResult Update(int id, Item updatedItem)
    {
        if (string.IsNullOrWhiteSpace(updatedItem.Name)) return BadRequest("Name is required");

        var item = Items.FirstOrDefault(x => x.Id == id);
        if (item == null) return NotFound("Item not found");

        item.Name = updatedItem.Name;

        return Ok(item);
    }

    // DELETE /api/items/4
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var item = Items.FirstOrDefault(x => x.Id == id);
        if (item == null) return NotFound("Item not found");

        Items.Remove(item);

        return NoContent();
    }
}
