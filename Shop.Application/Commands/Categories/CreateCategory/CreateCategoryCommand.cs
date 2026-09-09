using MediatR;
using Shop.Application.DTOs.CategoryDTOs;

namespace Shop.Application.Commands.Categories.CreateCategory;

public record CreateCategoryCommand(CategoryCreateDTO DTO) : IRequest<int?>;
