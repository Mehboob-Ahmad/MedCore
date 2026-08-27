using FluentValidation;
using MedicHp.Domain.Enums;
using MedicHp.Application.Features.Doctors.DTOs;

namespace MedicHp.Application.Features.Doctors.Commands.ConfigurePaymentMethods;

public class ConfigurePaymentMethodsCommandValidator : AbstractValidator<ConfigurePaymentMethodsCommand>
{
    public ConfigurePaymentMethodsCommandValidator()
    {
        RuleForEach(x => x.PaymentMethods).SetValidator(new PaymentMethodInputDtoValidator());
    }
}

public class PaymentMethodInputDtoValidator : AbstractValidator<PaymentMethodInputDto>
{
    public PaymentMethodInputDtoValidator()
    {
        RuleFor(x => x.PaymentMethodType).IsInEnum();

        When(x => x.PaymentMethodType == PaymentMethodType.BankTransfer, () =>
        {
            RuleFor(x => x.PaymentProvider).NotEmpty().WithMessage("Bank name is required for bank transfers.");
            RuleFor(x => x.AccountTitle).NotEmpty().WithMessage("Account title is required for bank transfers.");
            RuleFor(x => x.AccountNumber).NotEmpty().WithMessage("Account number is required for bank transfers.");
        });

        When(x => x.PaymentMethodType == PaymentMethodType.JazzCash || x.PaymentMethodType == PaymentMethodType.Easypaisa, () =>
        {
            RuleFor(x => x.AccountTitle).NotEmpty().WithMessage("Account title is required for mobile wallets.");
            RuleFor(x => x.AccountNumber).NotEmpty().WithMessage("Wallet number is required for mobile wallets.");
        });
        
        When(x => x.PaymentMethodType == PaymentMethodType.Other, () =>
        {
            RuleFor(x => x.PaymentProvider).NotEmpty().WithMessage("Provider name is required.");
            RuleFor(x => x.AccountTitle).NotEmpty().WithMessage("Account title is required.");
        });
    }
}
