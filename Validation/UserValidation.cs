using Connectify.Db.Model;
using FluentValidation;

namespace GachiHubBackend.Validation;

public class UserValidation : AbstractValidator<User>
{
    public UserValidation()
    {
        RuleFor(u => u.Login)
            .NotEmpty().WithMessage("Логин не указан")
            .MinimumLength(4).WithMessage("Минимальная длина логина 4 символа")
            .MaximumLength(15).WithMessage("Максимальная длина логина 15 символов");

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("Пароль не указан")
            .MinimumLength(8).WithMessage("Минимальная длина пароля 8 символов")
            .Matches(@"[A-Z]").WithMessage("Пароль должен содержать хотя-бы одну заглавную букву")
            .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("Пароль должен содержать спецсимвол");
    }
}