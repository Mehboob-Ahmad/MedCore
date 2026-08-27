using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Consultations.DTOs;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedicHp.Application.Features.Consultations.Queries.SearchConsultations;

public class SearchConsultationsQueryHandler : IRequestHandler<SearchConsultationsQuery, List<ConsultationSummaryDto>>
{
    private readonly IGenericRepository<Consultation> _consultationRepository;

    public SearchConsultationsQueryHandler(IGenericRepository<Consultation> consultationRepository)
    {
        _consultationRepository = consultationRepository;
    }

    public async Task<List<ConsultationSummaryDto>> Handle(SearchConsultationsQuery request, CancellationToken cancellationToken)
    {
        var query = _consultationRepository.GetQueryable().AsNoTracking()
            .Include(c => c.Doctor)
            .Include(c => c.Patient)
            .Where(c => c.DoctorId == request.DoctorId);

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var search = request.Query.ToLower();
            query = query.Where(c => 
                c.Diagnosis.ToLower().Contains(search) || 
                c.Patient.FirstName.ToLower().Contains(search) ||
                c.Patient.LastName.ToLower().Contains(search) ||
                c.ChiefComplaint.ToLower().Contains(search)
            );
        }

        if (request.DateFrom.HasValue)
            query = query.Where(c => c.CreatedAt >= request.DateFrom.Value);

        if (request.DateTo.HasValue)
            query = query.Where(c => c.CreatedAt <= request.DateTo.Value);

        var results = await query
            .OrderByDescending(c => c.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return results.Select(c => new ConsultationSummaryDto
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
