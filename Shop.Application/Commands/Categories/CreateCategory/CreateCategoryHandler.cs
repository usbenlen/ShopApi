using AutoMapper;
using MediatR;
using Shop.Application.Constants;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;

namespace Shop.Application.Commands.Categories.CreateCategory;

public class CreateCategoryHandler(
    ICategoryRepository categoryRepository,
    IMapper mapper,
    ICachingService cache)
    : IRequestHandler<CreateCategoryCommand, int?>
{
    public async Task<int?> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = mapper.Map<Category>(request.DTO);

        var result = await categoryRepository.CreateCategoryAsync(category);

        if (result.HasValue)
            await cache.InvalidateGroupAsync(CacheKeys.CategoriesGroup, cancellationToken);

        return result;
    }
}
