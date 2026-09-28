using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.OrderDTOs;
using Shop.Application.Interfaces.Services;
using System.Security.Claims;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController(
    IQueueService _queueService,
    IProductService _productService,
    IValidator<CreateOrderDTO> _validator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO dto, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

        var requestedProducts = dto.Products
            .GroupBy(x => x.ProductId)
            .Select(g => new OrderProductDTO
            {
                ProductId = g.Key,
                Count = g.Sum(x => x.Count)
            })
            .ToList();

        var products = new List<OrderMessageProductDTO>(requestedProducts.Count);

        foreach (var item in requestedProducts)
        {
            var product = await _productService.GetProductByIdAsync(item.ProductId, cancellationToken);

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
            OrderRequestId = Guid.NewGuid(),
            UserId = userId,
            Products = products,
            TotalPrice = totalPrice
        };

        await _queueService.PublishAsync(RabbitMqQueues.Orders, message, cancellationToken);

        return Accepted(new
        {
            message = "Order has been sent for processing",
            orderRequestId = message.OrderRequestId,
            totalPrice
        });
    }

}
