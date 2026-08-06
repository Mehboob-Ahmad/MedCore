using MedCore.Application.Common;
using MedCore.Application.Features.Records.DTOs;
using MedCore.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MedCore.Shared.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace MedCore.Application.Features.Records.Queries.GetConsultationSummary;

public class GetConsultationSummaryQueryHandler : IRequestHandler<GetConsultationSummaryQuery, ConsultationSummaryDto>
{
    private readonly IGenericRepository<Consultation> _consultationRepository;

    public GetConsultationSummaryQueryHandler(IGenericRepository<Consultation> consultationRepository)
    {
        _consultationRepository = consultationRepository;
    }

    public async Task<ConsultationSummaryDto> Handle(GetConsultationSummaryQuery request, CancellationToken cancellationToken)
    {
        var consultation = await _consultationRepository.FirstOrDefaultAsync(
            c => c.Id == request.ConsultationId && c.PatientId == request.UserId,
            include: q => q.Include(c => c.Doctor),
            cancellationToken);

        if (consultation == null) throw new NotFoundException(nameof(Consultation), request.ConsultationId);

        return new ConsultationSummaryDto
        {
            Id = consultation.Id,
            Date = consultation.CreatedAt,
            DoctorName = $"Dr. {consultation.Doctor?.FirstName} {consultation.Doctor?.LastName}",
            Symptoms = consultation.Symptoms ?? "",
            Diagnosis = consultation.Diagnosis ?? "",
            TreatmentPlan = consultation.TreatmentPlan ?? "",
            Notes = consultation.ClinicalNotes ?? ""
        };
    }
}
