using FluentValidation;
using Shop.Application.DTOs.ProductFeedbackDTOs;
using Shop.Domain.Enums;

namespace Shop.Application.Validators.ProductFeedback;

public class ProductFeedbackCreateValidator : AbstractValidator<ProductFeedbackCreateDTO>
{
    public ProductFeedbackCreateValidator()
    {
        RuleFor(feedback => feedback.ProductId)
            .GreaterThan(0)
            .WithMessage("ProductId має бути більшим за 0");

        RuleFor(feedback => feedback.Type)
            .IsInEnum()
            .WithMessage("Вказано некоректний тип відгуку");

        RuleFor(feedback => feedback.Message)
            .NotEmpty()
            .WithMessage("Повідомлення обов'язкове")
            .MaximumLength(1000)
            .WithMessage("Повідомлення не може бути довшим за 1000 символів");

        RuleFor(feedback => feedback.Rating)
            .InclusiveBetween(1, 5)
            .When(feedback => feedback.Rating.HasValue)
            .WithMessage("Рейтинг має бути від 1 до 5");
    }
}