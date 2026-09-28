using FluentValidation;
using Shop.Application.DTOs.UserDTOs;

namespace Shop.Application.Validators.User;

public class UserCreateValidator : AbstractValidator<UserCreateDTO>
{
    public UserCreateValidator()
    {
        RuleFor(user => user.Email)
            .NotEmpty()
            .WithMessage("Email обов'язковий")
            .EmailAddress()
            .WithMessage("Введіть коректний Email");

        RuleFor(user => user.Password)
            .NotEmpty()
            .WithMessage("Пароль обов'язковий")
            .MinimumLength(8)
            .WithMessage("Пароль має містити щонайменше 8 символів");
    }
}