using FluentValidation;
using MedCore.Application.Features.Auth.DTOs;

namespace MedCore.Application.Features.Auth.Validators;

public class RegisterDoctorValidator : AbstractValidator<RegisterDoctorDto>
{
    public RegisterDoctorValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MinimumLength(2).MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MinimumLength(2).MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
            
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords do not match.");
        RuleFor(x => x.SpecializationIds).NotEmpty().WithMessage("At least one specialization is required.");
        RuleFor(x => x.YearsOfExperience).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ConsultationFee).GreaterThan(0);
        RuleFor(x => x.LicenseNumber).NotEmpty();
        RuleFor(x => x.LicenseAuthority).NotEmpty();
        RuleFor(x => x.AcceptTerms).Equal(true).WithMessage("You must accept the terms and conditions.");
    }
}
