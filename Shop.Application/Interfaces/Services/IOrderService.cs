using Shop.Application.DTOs.OrderDTOs;

namespace Shop.Application.Interfaces.Services;

public interface IOrderService
{
    Task ProcessOrderAsync(OrderMessageDTO message, CancellationToken cancellationToken = default);
}
