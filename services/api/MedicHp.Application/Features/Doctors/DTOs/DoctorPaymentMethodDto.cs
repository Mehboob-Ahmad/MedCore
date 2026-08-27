using System;
using MedicHp.Domain.Enums;

namespace MedicHp.Application.Features.Doctors.DTOs;

public class DoctorPaymentMethodDto
{
    public Guid Id { get; set; }
    public PaymentMethodType PaymentMethodType { get; set; }
    public string PaymentMethodTypeName => PaymentMethodType.ToString();
    public string? PaymentProvider { get; set; }
    public string? AccountTitle { get; set; }
    public string? AccountNumber { get; set; }
    public string? MaskedAccountNumber { get; set; }
    public string? IBAN { get; set; }
    public string? MaskedIBAN { get; set; }
    public bool IsActive { get; set; }
}

public class PaymentMethodInputDto
{
    public PaymentMethodType PaymentMethodType { get; set; }
    public string? PaymentProvider { get; set; }
    public string? AccountTitle { get; set; }
    public string? AccountNumber { get; set; }
    public string? IBAN { get; set; }
    public bool IsActive { get; set; } = true;
}
