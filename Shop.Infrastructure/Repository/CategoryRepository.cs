using Microsoft.EntityFrameworkCore;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repository;

public class CategoryRepository(ShopDbContext _context) : ICategoryRepository
{
    public async Task<int?> CreateCategoryAsync(Category category, CancellationToken cancellationToken = default)
    {
        await _context.Categories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return category.Id;
    }

    public async Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
    }

    public async Task<Category?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Category?> GetCategoryForUpdateAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories.FindAsync(id, cancellationToken);
        if (category == null) return false;

        _context.Categories.Remove(category);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> UpdateCategoryAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<Category>> GetParentCategoriesAsync(int id, CancellationToken cancellationToken = default)
    {
        var categories = await _context.Categories.AsNoTracking().ToListAsync(cancellationToken);
        var dict = categories.ToDictionary(x => x.Id);
        var result = new List<Category>();

        if (!dict.TryGetValue(id, out var current)) return result;

        while (current.ParentId != null)
        {
            if (!dict.TryGetValue(current.ParentId.Value, out var parent)) break;

            result.Add(parent);
            current = parent;
        }

        result.Reverse(); //від кореня до поточної
        return result;
    }

    public async Task<IReadOnlyList<Category>> GetChildCategoriesAsync(int id, CancellationToken cancellationToken = default)
    {
        var categories = await _context.Categories.AsNoTracking().ToListAsync(cancellationToken);
        var result = new List<Category>();

        void FindChildren(int parentId, CancellationToken cancellationToken = default)
        {
            var children = categories.Where(x => x.ParentId == parentId).ToList();

            foreach (var child in children)
            {
                result.Add(child);
                FindChildren(child.Id, cancellationToken);
            }
        }

        FindChildren(id, cancellationToken);
        return result;
    }

    public async Task<Category?> GetCategoryTreeAsync(int id, CancellationToken cancellationToken = default)
    {
        var categories = await _context.Categories.AsNoTracking().ToListAsync(cancellationToken);
        var dict = categories.ToDictionary(x => x.Id);

        var lookup = categories.ToLookup(c => c.ParentId);
        foreach (var category in categories) category.SubCategories = lookup[category.Id].ToList();

        dict.TryGetValue(id, out var root);
        return root;
    }
}
