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

    public async Task<int?> CreateCategoryAsync(CategoryCreateDTO dto)
    {
        var category = _mapper.Map<Category>(dto);

        var result = await _repository.CreateCategoryAsync(category);

        if (result.HasValue) await _cache.InvalidateGroupAsync(CacheKeys.CategoriesGroup);

        return result;
    }

    public async Task<IReadOnlyList<CategoryReadDTO>> GetCategoriesAsync()
    {
        return await _cache.GetOrCreateAsync(
            CacheKeys.AllCategories,

            async () =>
            {
                var categories = await _repository.GetCategoriesAsync();
                return _mapper.Map<List<CategoryReadDTO>>(categories);
            },

            CacheExpiration,
            CacheKeys.CategoriesGroup) ?? [];
    }

    public async Task<CategoryReadDTO?> GetCategoryByIdAsync(int id)
    {
        return await _cache.GetOrCreateAsync(
            CacheKeys.Category(id),

            async () =>
            {
                var category = await _repository.GetCategoryByIdAsync(id);
                if (category is null) return null;
                return _mapper.Map<CategoryReadDTO>(category);
            },

            CacheExpiration,
            CacheKeys.CategoriesGroup);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var result = await _repository.DeleteCategoryAsync(id);

        if (result) await _cache.InvalidateGroupAsync(CacheKeys.CategoriesGroup);

        return result;
    }

    public async Task<bool> UpdateCategoryAsync(int id, CategoryUpdateDTO dto)
    {
        var category = await _repository.GetCategoryForUpdateAsync(id);
        if (category is null) return false;

        _mapper.Map(dto, category);

        var result = await _repository.UpdateCategoryAsync();
        if (result) await _cache.InvalidateGroupAsync(CacheKeys.CategoriesGroup);

        return result;
    }

    public async Task<IReadOnlyList<CategoryReadDTO>> GetParentCategoriesAsync(int id)
    {
        return await _cache.GetOrCreateAsync(
            CacheKeys.ParentCategories(id),

            async () =>
            {
                var categories = await _repository.GetParentCategoriesAsync(id);
                return _mapper.Map<List<CategoryReadDTO>>(categories);
            },

            CacheExpiration,
            CacheKeys.CategoriesGroup) ?? [];
    }

    public async Task<IReadOnlyList<CategoryReadDTO>> GetChildCategoriesAsync(int id)
    {
        return await _cache.GetOrCreateAsync(
            CacheKeys.ChildCategories(id),

            async () =>
            {
                var categories = await _repository.GetChildCategoriesAsync(id);
                return _mapper.Map<List<CategoryReadDTO>>(categories);
            },

            CacheExpiration,
            CacheKeys.CategoriesGroup) ?? [];
    }

    public async Task<CategoryTreeDTO?> GetCategoryTreeAsync(int id)
    {
        return await _cache.GetOrCreateAsync(
            CacheKeys.CategoryTree(id),

            async () =>
            {
                var category = await _repository.GetCategoryTreeAsync(id);
                if (category is null) return null;
                return _mapper.Map<CategoryTreeDTO>(category);
            },

            CacheExpiration,
            CacheKeys.CategoriesGroup);
    }
}
