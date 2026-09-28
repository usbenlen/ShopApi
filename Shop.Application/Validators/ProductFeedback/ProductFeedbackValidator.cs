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
            .Cascade(CascadeMode.Stop)
            .Must(message => !string.IsNullOrWhiteSpace(message))
            .WithMessage("Повідомлення обов'язкове")
            .MaximumLength(1000)
            .WithMessage("Повідомлення не може бути довшим за 1000 символів");

        RuleFor(feedback => feedback.Rating)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("Для відгуку необхідно вказати рейтинг")
            .InclusiveBetween(1, 5)
            .WithMessage("Рейтинг має бути від 1 до 5")
            .When(feedback => feedback.Type == ProductFeedbackType.Review);

        RuleFor(feedback => feedback.Rating)
            .Null()
            .WithMessage("Для питання рейтинг не вказується")
            .When(feedback => feedback.Type == ProductFeedbackType.Question);
    }
}