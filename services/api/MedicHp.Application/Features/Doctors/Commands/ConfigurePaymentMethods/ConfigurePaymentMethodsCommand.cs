using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MedicHp.Application.Features.Doctors.DTOs;
using MediatR;

namespace MedicHp.Application.Features.Doctors.Commands.ConfigurePaymentMethods;

public class ConfigurePaymentMethodsCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid UserId { get; set; }

    public List<PaymentMethodInputDto> PaymentMethods { get; set; } = new();
}
