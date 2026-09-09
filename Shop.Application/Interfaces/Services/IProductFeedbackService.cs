using Shop.Application.DTOs.ProductFeedbackDTOs;

namespace Shop.Application.Interfaces.Services;

public interface IProductFeedbackService
{
    Task CreateAsync(int productId, Guid userId, ProductFeedbackCreateDTO dto);
}
