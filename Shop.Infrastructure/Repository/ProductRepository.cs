using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repository;

/// <summary>
/// Репозиторій для роботи з продуктами
/// </summary>
public class ProductRepository(ShopDbContext _context) : IProductRepository
{
    /// <inheritdoc/>
    public async Task<int?> CreateProductAsync(Product product)
    {
        await _context.Products.AddAsync(product);

        await _context.SaveChangesAsync();

        return product.Id;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Product>> GetProductsAsync()
    {
        return await _context.Products
            .Include(x => x.Images)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products
            .Include(x => x.Images)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
        if (product == null) return false;

        _context.Products.Remove(product);

        await _context.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateProductAsync(Product product)
    {
        var existing = await _context.Products
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == product.Id);

        if (existing == null) return false;

        existing.Name = product.Name;
        existing.Price = product.Price;
        existing.Description = product.Description;
        existing.StockQty = product.StockQty;
        existing.IsActive = product.IsActive;
        existing.CategoryId = product.CategoryId;

        //видаляємо старі фото
        _context.ProductImages.RemoveRange(existing.Images);
        existing.Images.Clear();

        //додаєм нові
        foreach (var image in product.Images) existing.Images.Add(image);

        await _context.SaveChangesAsync();

        return true;
    }
}