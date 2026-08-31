using Shop.Application.DTOs.OrderDTOs;

namespace Shop.Application.Interfaces.Repository;

public interface IOrderRepository
{
    Task<OrderProcessingResultDTO> CreateOrderAsync(Guid orderRequestId, Guid userId, IReadOnlyList<OrderMessageProductDTO> products, CancellationToken cancellationToken = default);
}
