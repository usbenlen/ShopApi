using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using Shop.Application.Configuration;
using Shop.Application.Constants;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;

namespace Shop.Application.Queries.Category.GetCategoryBySlug;

public class GetCategoryBySlugHandler(
    ICategoryRepository categoryRepository,
    IMapper mapper,
    ICachingService cache,
    IOptions<CachingSettings> cacheSettings)
    : IRequestHandler<GetCategoryBySlugQuery, CategoryReadDTO?>
{
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(cacheSettings.Value.Categories.ExpirationMinutes);

    public async Task<CategoryReadDTO?> Handle(GetCategoryBySlugQuery request, CancellationToken cancellationToken)
    {
        return await cache.GetOrCreateAsync(
            CacheKeys.CategoryBySlug(request.Slug),

            async ct =>
            {
                var category = await categoryRepository.GetCategoryBySlugAsync(request.Slug, ct);
                if (category is null) return null;

                return mapper.Map<CategoryReadDTO>(category);
            },

            _cacheExpiration,
            CacheKeys.CategoriesGroup,
            cancellationToken);
    }
}
