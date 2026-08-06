using System;
using MediatR;

namespace MedCore.Application.Features.Productivity.Commands.CopyPreviousPrescription;

public class CopyPreviousPrescriptionCommand : IRequest<bool>
{
    public Guid SourceConsultationId { get; set; }
    public Guid TargetConsultationId { get; set; }
}
