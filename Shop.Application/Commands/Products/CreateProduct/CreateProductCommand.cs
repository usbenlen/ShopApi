using MediatR;
using Shop.Application.DTOs.ProductDTOs;

namespace Shop.Application.Commands.Products.CreateProduct;

public record CreateProductCommand(ProductCreateDTO DTO) : IRequest<int?>;
