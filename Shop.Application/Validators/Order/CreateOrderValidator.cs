using FluentValidation;
using Shop.Application.DTOs.OrderDTOs;

namespace Shop.Application.Validators.Order;

public class CreateOrderValidator : AbstractValidator<CreateOrderDTO>
{
    public CreateOrderValidator()
    {
        RuleFor(order => order.Products)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("Список товарів обов'язковий")
            .NotEmpty()
            .WithMessage("Замовлення повинно містити хоча б один товар")
            .Must(products => products.Count <= 50)
            .WithMessage("Замовлення не може містити більше 50 товарів");

        RuleForEach(order => order.Products)
            .SetValidator(new OrderProductValidator());
    }
}
