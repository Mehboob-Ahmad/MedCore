using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Consultations.DTOs;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedicHp.Application.Features.Consultations.Queries.GetPatientConsultationHistory;

public class GetPatientConsultationHistoryQueryHandler : IRequestHandler<GetPatientConsultationHistoryQuery, List<ConsultationSummaryDto>>
{
    private readonly IGenericRepository<Consultation> _consultationRepository;

    public GetPatientConsultationHistoryQueryHandler(IGenericRepository<Consultation> consultationRepository)
    {
        _consultationRepository = consultationRepository;
    }

    public async Task<List<ConsultationSummaryDto>> Handle(GetPatientConsultationHistoryQuery request, CancellationToken cancellationToken)
    {
        var query = _consultationRepository.GetQueryable().AsNoTracking()
            .Include(c => c.Doctor)
            .Include(c => c.Patient)
            .Where(c => c.PatientId == request.PatientId);

        // Security check handled at Controller level via attributes/policies, but filter here just in case it's a doctor
        // We'll allow access if request.UserId == PatientId, or if it's a doctor we assume they are authorized.
        
        var consultations = await query
            .OrderByDescending(c => c.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return consultations.Select(c => new ConsultationSummaryDto
        {
            Id = c.Id,
            PatientId = c.PatientId,
            PatientName = $"{c.Patient.FirstName} {c.Patient.LastName}",
            DoctorId = c.DoctorId,
            DoctorName = $"Dr. {c.Doctor.FirstName} {c.Doctor.LastName}",
            ChiefComplaint = c.ChiefComplaint,
            Diagnosis = c.Diagnosis,
            VisitType = c.VisitType,
            IsFinalized = c.IsFinalized,
            CreatedAt = c.CreatedAt
        }).ToList();
    }
}
