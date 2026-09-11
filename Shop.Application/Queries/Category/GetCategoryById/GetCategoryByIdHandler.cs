using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using Shop.Application.Configuration;
using Shop.Application.Constants;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;

namespace Shop.Application.Queries.Category.GetCategoryById;

public class GetCategoryByIdHandler(
    ICategoryRepository categoryRepository,
    IMapper mapper,
    ICachingService cache,
    IOptions<CachingSettings> cacheSettings)
    : IRequestHandler<GetCategoryByIdQuery, CategoryReadDTO?>
{
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(cacheSettings.Value.Categories.ExpirationMinutes);

    public async Task<CategoryReadDTO?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        return await cache.GetOrCreateAsync(
            CacheKeys.Category(request.id),

            async ct =>
            {
                var category = await categoryRepository.GetCategoryByIdAsync(request.id, ct);
                if (category is null)return null;

                return mapper.Map<CategoryReadDTO>(category);
            },

            _cacheExpiration,
            CacheKeys.CategoriesGroup,
            cancellationToken);
    }
}
