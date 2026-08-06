using FluentValidation;
using MedCore.Application.Features.Auth.DTOs;

namespace MedCore.Application.Features.Auth.Validators;

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
