using FluentValidation;
using MedicHp.Application.Features.Auth.DTOs;

namespace MedicHp.Application.Features.Auth.Validators;

public class InviteAdminValidator : AbstractValidator<InviteAdminDto>
{
    public InviteAdminValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");
    }
}
