using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Infrastructure.Repository;

public class CategoryRepository(ShopDbContext _context) : ICategoryRepository
{
    public async Task<int?> CreateCategoryAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return category.Id;
    }

    public async Task<IReadOnlyList<Category>> GetCategoriesAsync()
    {
        return await _context.Categories
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return false;

        _context.Categories.Remove(category);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateCategoryAsync(Category category)
    {
        var existing = await _context.Categories.FindAsync(category.Id);
        if (existing == null) return false;

        existing.Name = category.Name;
        existing.Slug = category.Slug;
        existing.Description = category.Description;
        existing.ImageURL = category.ImageURL;
        existing.ParentId = category.ParentId;

        await _context.SaveChangesAsync();

        return true;
    }
}
