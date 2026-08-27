using MedicHp.Domain.Common;
using MedicHp.Domain.Enums;

namespace MedicHp.Domain.Entities.Clinical;

/// <summary>
/// Represents a payment method configured by a doctor for receiving consultation fees.
/// A doctor can have multiple active payment methods (e.g., Cash + BankTransfer + JazzCash).
/// </summary>
public class DoctorPaymentMethod : SoftDeleteEntity
{
    public Guid DoctorProfileId { get; set; }
    public DoctorProfile DoctorProfile { get; set; } = null!;

    /// <summary>
    /// The type of payment method (Cash, BankTransfer, JazzCash, Easypaisa, Other).
    /// </summary>
    public PaymentMethodType PaymentMethodType { get; set; }

    /// <summary>
    /// Payment provider name. Examples:
    /// - BankTransfer: "Meezan Bank", "HBL", "UBL"
    /// - JazzCash: "JazzCash"
    /// - Easypaisa: "Easypaisa"
    /// - Other: custom provider name
    /// - Cash: null (not applicable)
    /// </summary>
    public string? PaymentProvider { get; set; }

    /// <summary>
    /// Account holder name / name on account.
    /// Not applicable for Cash.
    /// </summary>
    public string? AccountTitle { get; set; }

    /// <summary>
    /// Bank account number or mobile wallet number.
    /// Treated as sensitive financial information — masked in patient-facing responses.
    /// Not applicable for Cash.
    /// </summary>
    public string? AccountNumber { get; set; }

    /// <summary>
    /// International Bank Account Number. Only applicable for BankTransfer.
    /// Treated as sensitive financial information — masked in patient-facing responses.
    /// </summary>
    public string? IBAN { get; set; }

    /// <summary>
    /// Whether this payment method is currently active and visible to patients.
    /// </summary>
    public bool IsActive { get; set; } = true;
}
