using AutoMapper;
using Microsoft.Extensions.Options;
using Shop.Application.Configuration;
using Shop.Application.Constants;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;

namespace Shop.Application.Services;

public class CategoryService(ICategoryRepository _repository, IMapper _mapper, ICachingService _cache, IOptions<CachingSettings> _cacheSettings) : ICategoryService
{
    private readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(_cacheSettings.Value.Categories.ExpirationMinutes);

    public async Task<int?> CreateCategoryAsync(CategoryCreateDTO dto, CancellationToken cancellationToken = default)
    {
        var category = _mapper.Map<Category>(dto);

        var result = await _repository.CreateCategoryAsync(category, cancellationToken);

        if (result.HasValue) await _cache.InvalidateGroupAsync(CacheKeys.CategoriesGroup, cancellationToken);

        return result;
    }

    public async Task<IReadOnlyList<CategoryReadDTO>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _cache.GetOrCreateAsync(
            CacheKeys.AllCategories,

            async ct =>
            {
                var categories = await _repository.GetCategoriesAsync(ct);
                return _mapper.Map<List<CategoryReadDTO>>(categories);
            },

            CacheExpiration,
            CacheKeys.CategoriesGroup,
            cancellationToken) ?? [];
    }

    public async Task<CategoryReadDTO?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _cache.GetOrCreateAsync(
            CacheKeys.Category(id),

            async ct =>
            {
                var category = await _repository.GetCategoryByIdAsync(id, ct);
                if (category is null) return null;
                return _mapper.Map<CategoryReadDTO>(category);
            },

            CacheExpiration,
            CacheKeys.CategoriesGroup,
            cancellationToken);
    }

    public async Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = await _repository.DeleteCategoryAsync(id, cancellationToken);

        if (result) await _cache.InvalidateGroupAsync(CacheKeys.CategoriesGroup, cancellationToken);

        return result;
    }

    public async Task<bool> UpdateCategoryAsync(int id, CategoryUpdateDTO dto, CancellationToken cancellationToken = default)
    {
        var category = await _repository.GetCategoryForUpdateAsync(id, cancellationToken);
        if (category is null) return false;

        _mapper.Map(dto, category);

        var result = await _repository.UpdateCategoryAsync(cancellationToken);
        if (result) await _cache.InvalidateGroupAsync(CacheKeys.CategoriesGroup, cancellationToken);

        return result;
    }

    public async Task<IReadOnlyList<CategoryReadDTO>> GetParentCategoriesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _cache.GetOrCreateAsync(
            CacheKeys.ParentCategories(id),

            async ct =>
            {
                var categories = await _repository.GetParentCategoriesAsync(id, ct);
                return _mapper.Map<List<CategoryReadDTO>>(categories);
            },

            CacheExpiration,
            CacheKeys.CategoriesGroup,
            cancellationToken) ?? [];
    }

    public async Task<IReadOnlyList<CategoryReadDTO>> GetChildCategoriesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _cache.GetOrCreateAsync(
            CacheKeys.ChildCategories(id),

            async ct =>
            {
                var categories = await _repository.GetChildCategoriesAsync(id, ct);
                return _mapper.Map<List<CategoryReadDTO>>(categories);
            },

            CacheExpiration,
            CacheKeys.CategoriesGroup,
            cancellationToken) ?? [];
    }

    public async Task<CategoryTreeDTO?> GetCategoryTreeAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _cache.GetOrCreateAsync(
            CacheKeys.CategoryTree(id),

            async ct =>
            {
                var category = await _repository.GetCategoryTreeAsync(id, ct);
                if (category is null) return null;
                return _mapper.Map<CategoryTreeDTO>(category);
            },

            CacheExpiration,
            CacheKeys.CategoriesGroup,
            cancellationToken);
    }
}
