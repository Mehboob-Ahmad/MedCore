using System;
using System.Collections.Generic;
using MedCore.Application.Features.Consultations.DTOs;
using MediatR;

namespace MedCore.Application.Features.Consultations.Commands.SavePrescription;

public class SavePrescriptionCommand : IRequest<Guid>
{
    public Guid ConsultationId { get; set; }
    public Guid DoctorId { get; set; }
    public string? Notes { get; set; }
    public List<PrescriptionItemDto> Items { get; set; } = new();
}
