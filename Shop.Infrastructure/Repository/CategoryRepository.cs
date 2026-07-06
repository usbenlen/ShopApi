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
}
