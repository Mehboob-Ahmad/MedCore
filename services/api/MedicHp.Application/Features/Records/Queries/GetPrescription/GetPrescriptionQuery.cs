using MedicHp.Application.Features.Records.DTOs;
using MediatR;
using System;

namespace MedicHp.Application.Features.Records.Queries.GetPrescription;

public class GetPrescriptionQuery : IRequest<PrescriptionDto>
{
    public Guid UserId { get; set; }
    public Guid PrescriptionId { get; set; }

    public GetPrescriptionQuery(Guid userId, Guid prescriptionId)
    {
        UserId = userId;
        PrescriptionId = prescriptionId;
    }
}
