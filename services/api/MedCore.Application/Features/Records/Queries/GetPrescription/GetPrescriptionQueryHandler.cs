using MedCore.Application.Common;
using MedCore.Application.Features.Records.DTOs;
using MedCore.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MedCore.Shared.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace MedCore.Application.Features.Records.Queries.GetPrescription;

public class GetPrescriptionQueryHandler : IRequestHandler<GetPrescriptionQuery, PrescriptionDto>
{
    private readonly IGenericRepository<Prescription> _prescriptionRepository;

    public GetPrescriptionQueryHandler(IGenericRepository<Prescription> prescriptionRepository)
    {
        _prescriptionRepository = prescriptionRepository;
    }

    public async Task<PrescriptionDto> Handle(GetPrescriptionQuery request, CancellationToken cancellationToken)
    {
        var prescription = await _prescriptionRepository.FirstOrDefaultAsync(
            p => p.Id == request.PrescriptionId && p.PatientId == request.UserId,
            include: q => q.Include(p => p.Doctor).Include(p => p.Items),
            cancellationToken);

        if (prescription == null) throw new NotFoundException(nameof(Prescription), request.PrescriptionId);

        var item = prescription.Items.FirstOrDefault(); // Mocking single item for now or returning first
        
        return new PrescriptionDto
        {
            Id = prescription.Id,
            IssueDate = prescription.IssuedAt,
            DoctorName = $"Dr. {prescription.Doctor?.FirstName} {prescription.Doctor?.LastName}",
            MedicationName = item?.MedicationName ?? "Unknown",
            Dosage = item?.Dosage ?? "",
            Frequency = item?.Frequency ?? "",
            Duration = item?.Duration ?? "",
            Instructions = item?.Instructions ?? ""
        };
    }
}
