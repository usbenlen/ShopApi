using FluentValidation;
using Shop.Application.DTOs.OrderDTOs;

namespace Shop.Application.Validators.Order;

public class OrderProductValidator : AbstractValidator<OrderProductDTO>
{
    public OrderProductValidator()
    {
        RuleFor(product => product.ProductId)
            .GreaterThan(0)
            .WithMessage("ProductId має бути більшим за 0");

        RuleFor(product => product.Count)
            .GreaterThan(0)
            .WithMessage("Кількість товару має бути більшою за 0");
    }
}