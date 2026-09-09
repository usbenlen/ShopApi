using MediatR;
using Shop.Application.DTOs.CategoryDTOs;

namespace Shop.Application.Queries.Category.GetCategoryBySlug;

public record GetCategoryBySlugQuery(string Slug) : IRequest<CategoryReadDTO?>;
