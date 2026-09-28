using FluentValidation;
using Shop.Application.DTOs.CategoryDTOs;

namespace Shop.Application.Validators.Category;

public class CategoryValidator : AbstractValidator<CategoryCreateDTO>
{
    public CategoryValidator()
    {
        RuleFor(category => category.Name)
            .NotEmpty()
            .WithMessage("Ім'я категорії обов'язкове")
            .MaximumLength(10)
            .WithMessage("Ім'я категорії не може бути довшим за 10 символів");

        RuleFor(category => category.Slug)
            .NotEmpty()
            .WithMessage("Slug категорії обов'язковий")
            .MaximumLength(100)
            .WithMessage("Slug не може бути довшим за 100 символів")
            .Matches(@"^[a-zA-Z0-9_-]+$")
            .WithMessage("Slug повинен містити латинські літери, цифри та символи");

        RuleFor(category => category.ParentId)
            .GreaterThan(0)
            .When(category => category.ParentId.HasValue)
            .WithMessage("ParentId має бути більшим за 0");

        RuleFor(category => category.Description)
            .MaximumLength(1000)
            .WithMessage("Опис не може бути довшим за 1000 символів");

        //RuleFor(category => category.ImageURL)
        //    .Must(BeValidUrl)
        //    .When(category => !string.IsNullOrWhiteSpace(category.ImageURL))
        //    .WithMessage("ImageURL має бути коректним URL");
    }

    //private bool BeValidUrl(string? url)
    //{
    //    return Uri.TryCreate(url, UriKind.Absolute, out var result)
    //           && (result.Scheme == Uri.UriSchemeHttp ||
    //               result.Scheme == Uri.UriSchemeHttps);
    //}
}
