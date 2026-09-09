using MediatR;
using Shop.Application.DTOs.CategoryDTOs;

namespace Shop.Application.Queries.Category.GetCategoryById;

public record GetCategoryByIdQuery(int id) : IRequest<CategoryReadDTO?>;
