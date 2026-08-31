using Microsoft.EntityFrameworkCore;
using Shop.Application.DTOs.OrderDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Enums;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repository;

public class OrderRepository(ShopDbContext context) : IOrderRepository
{
    public async Task<OrderProcessingResultDTO> CreateOrderAsync(Guid orderRequestId, Guid userId, IReadOnlyList<OrderMessageProductDTO> products, CancellationToken cancellationToken = default)
    {
        if (orderRequestId == Guid.Empty)
            throw new ArgumentException("Order request id cannot be empty", nameof(orderRequestId));

        if (userId == Guid.Empty)
            throw new ArgumentException("User id cannot be empty", nameof(userId));

        if (products.Count == 0)
            throw new ArgumentException("Order must contain at least one product", nameof(products));

        var userExists = await context.Users
            .AsNoTracking()
            .AnyAsync(x => x.Id == userId && x.IsActive, cancellationToken);

        if (!userExists)
            throw new InvalidOperationException($"User {userId} does not exist or is inactive");

        var existingOrder = await GetOrderByRequestIdAsync(orderRequestId,cancellationToken);

        if (existingOrder is not null)
            return BuildExistingOrderResult(existingOrder);


        // об'єднання однакових ProductId
        var requestedProducts = products
            .GroupBy(x => x.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                Count = g.Sum(x => x.Count)
            })
            .ToList();

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var productIds = requestedProducts
                .Select(x => x.ProductId)
                .ToList();

            var dbProducts = await context.Products
                .Where(x => productIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);

            var unavailableProducts = new List<OrderProcessingProductDTO>();
            var availableProducts = new List<(Product Product, int Count)>();

            foreach (var requested in requestedProducts)
            {
                if (!dbProducts.TryGetValue(requested.ProductId, out var product))
                {
                    unavailableProducts.Add(new OrderProcessingProductDTO
                    {
                        ProductId = requested.ProductId,
                        ProductName = "Unknown product",
                        Count = requested.Count
                    });

                    continue;
                }

                if (!product.IsActive || product.StockQty < requested.Count)
                {
                    unavailableProducts.Add(new OrderProcessingProductDTO
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Price = product.Price,
                        Count = requested.Count
                    });

                    continue;
                }

                availableProducts.Add((product, requested.Count));
            }

            // якщо хоч чогось не вистачає то все відміняєм і не створюєм
            if (unavailableProducts.Count > 0)
            {
                await transaction.RollbackAsync(cancellationToken);

                return new OrderProcessingResultDTO
                {
                    Success = false,
                    WaitingForStock = true,
                    UnavailableProducts = unavailableProducts
                };
            }

            foreach (var item in availableProducts)
            {
                var affectedRows =
                    await context.Products
                        .Where(x => x.Id == item.Product.Id && x.IsActive && x.StockQty >= item.Count)
                        .ExecuteUpdateAsync(setters => setters
                                .SetProperty(x => x.StockQty, x => x.StockQty - item.Count), cancellationToken);

                if (affectedRows != 1)
                {
                    await transaction.RollbackAsync(cancellationToken);

                    return new OrderProcessingResultDTO
                    {
                        Success = false,
                        WaitingForStock = true,
                        UnavailableProducts =
                        [
                            new OrderProcessingProductDTO
                            {
                                ProductId = item.Product.Id,
                                ProductName = item.Product.Name,
                                Price = item.Product.Price,
                                Count = item.Count
                            }
                        ]
                    };
                }
            }

            var orderDetails = new List<OrderDetail>();

            decimal totalPrice = 0m;

            foreach (var item in availableProducts)
            {
                totalPrice += item.Product.Price * item.Count;

                orderDetails.Add(new OrderDetail
                {
                    ProductId = item.Product.Id,
                    Price = item.Product.Price,
                    Count = item.Count
                });
            }

            var order = new Order
            {
                OrderRequestId = orderRequestId,
                UserId = userId,
                Status = OrderStatus.Pending,
                Paid = false,
                OrderDetails = orderDetails
            };

            await context.Orders.AddAsync(order, cancellationToken);

            try
            {
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync(cancellationToken);

                var duplicateOrder = await GetOrderByRequestIdAsync(orderRequestId, cancellationToken);

                if (duplicateOrder is not null)
                    return BuildExistingOrderResult(duplicateOrder);

                throw;
            }



            await transaction.CommitAsync(cancellationToken);

            var resultProducts = availableProducts
                .Select(item => new OrderProcessingProductDTO
                {
                    ProductId = item.Product.Id,
                    ProductName = item.Product.Name,
                    Price = item.Product.Price,
                    Count = item.Count
                })
                .ToList();

            return new OrderProcessingResultDTO
            {
                Success = true,
                WaitingForStock = false,
                OrderId = order.Id,
                TotalPrice = totalPrice,
                Products = resultProducts
            };
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task<Order?> GetOrderByRequestIdAsync(Guid orderRequestId, CancellationToken cancellationToken)
    {
        return await context.Orders
            .AsNoTracking()
            .Include(x => x.OrderDetails)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.OrderRequestId == orderRequestId, cancellationToken);
    }


    private OrderProcessingResultDTO BuildExistingOrderResult(Order order)
    {
        var products = order.OrderDetails
            .Select(detail => new OrderProcessingProductDTO
            {
                ProductId = detail.ProductId,
                ProductName = detail.Product.Name,
                Price = detail.Price,
                Count = detail.Count
            })
            .ToList();

        var totalPrice = products.Sum(x => x.Price * x.Count);

        return new OrderProcessingResultDTO
        {
            Success = true,
            AlreadyProcessed = true,
            WaitingForStock = false,
            OrderId = order.Id,
            TotalPrice = totalPrice,
            Products = products
        };
    }
}
