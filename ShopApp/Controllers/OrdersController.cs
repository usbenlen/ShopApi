using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.OrderDTOs;
using Shop.Application.Interfaces.Services;
using System.Security.Claims;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController(IQueueService _queueService, IProductService _productService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO dto, CancellationToken cancellationToken)
    {
        if (dto.Products is null || dto.Products.Count == 0)
            return BadRequest("Order must contain at least one product");

        // дістаю з jwt GUID а потім його парсю
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

        var products = new List<OrderMessageProductDTO>();

        foreach (var item in dto.Products)
        {
            if (item.Count <= 0)
                return BadRequest($"Invalid count for product {item.ProductId}");

            var product = await _productService.GetProductByIdAsync(item.ProductId);

            if (product is null)
                return BadRequest($"Product {item.ProductId} not found");

            if (!product.IsActive)
                return BadRequest($"Product {item.ProductId} is inactive");

            if (product.StockQty < item.Count)
                return BadRequest($"Not enough stock for product {item.ProductId}");

            products.Add(new OrderMessageProductDTO
            {
                ProductId = product.Id,
                Count = item.Count,
                Price = product.Price
            });
        }

        var totalPrice = products.Sum(x => x.Price * x.Count);

        var message = new OrderMessageDTO
        {
            UserId = userId,
            Products = products,
            TotalPrice = totalPrice
        };

        await _queueService.PublishAsync(RabbitMqQueues.Orders, message, cancellationToken);

        return Accepted(new
        {
            message = "Order has been sent for processing",
            totalPrice
        });
    }
}
