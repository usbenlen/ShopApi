using FluentValidation;
using Shop.Application.DTOs.ProductDTOs;

namespace Shop.Application.Validators.Product;

public class ProductValidator : AbstractValidator<ProductCreateDTO>
{
    public ProductValidator()
    {
        RuleFor(product => product.Name)
            .NotEmpty()
            .WithMessage("Назва продукту обов'язкова")
            .MaximumLength(200)
            .WithMessage("Назва продукту не може бути довшою за 200 символів");

        RuleFor(product => product.Price)
            .GreaterThan(0)
            .WithMessage("Ціна продукту має бути більшою за 0");

        RuleFor(product => product.OldPrice)
            .GreaterThan(0)
            .When(product => product.OldPrice.HasValue)
            .WithMessage("Стара ціна має бути більшою за 0");

        RuleFor(product => product.OldPrice)
            .GreaterThan(product => product.Price)
            .When(product => product.OldPrice.HasValue)
            .WithMessage("Стара ціна має бути більшою за поточну ціну");

        RuleFor(product => product.Description)
            .MaximumLength(2000)
            .WithMessage("Опис не може бути довшим за 2000 символів");

        RuleFor(product => product.StockQty)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Кількість товару не може бути від'ємною");

        RuleFor(product => product.CategoryId)
            .GreaterThan(0)
            .WithMessage("CategoryId має бути більшим за 0");

        RuleFor(product => product.Images)
            .Must(images => images.Count <= 10)
            .WithMessage("Продукт може мати не більше 10 фотографій");

        RuleForEach(product => product.Images)
            .Must(BeValidUrl)
            .WithMessage("URL фотографії має бути коректним URL");

        RuleFor(product => product.IsDiscounted)
            .Equal(true)
            .When(product => product.OldPrice.HasValue)
            .WithMessage("Якщо вказана стара ціна, продукт має мати активну знижку");

        RuleFor(product => product.OldPrice)
            .NotNull()
            .When(product => product.IsDiscounted)
            .WithMessage("Для товару зі знижкою необхідно вказати стару ціну");
    }

    private bool BeValidUrl(string? url)
    {
        return !string.IsNullOrWhiteSpace(url)
               && Uri.TryCreate(url, UriKind.Absolute, out var result)
               && (result.Scheme == Uri.UriSchemeHttp ||
                   result.Scheme == Uri.UriSchemeHttps);
    }
}
