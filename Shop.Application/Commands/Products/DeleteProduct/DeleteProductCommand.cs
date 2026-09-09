using MediatR;

namespace Shop.Application.Commands.Products.DeleteProduct;

public record DeleteProductCommand(int id) : IRequest<int?>;
