using MediatR;
using Shop.Application.Interfaces.Repository;

namespace Shop.Application.Commands.Products.DeleteProduct;

public class DeleteProductHandler(IProductRepository _repository) : IRequestHandler<DeleteProductCommand, int?>
{
    public async Task<int?> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        bool deleted = await _repository.DeleteProductAsync(request.id, cancellationToken);
        if (!deleted) return null;

        return request.id;
    }
}
