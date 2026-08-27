using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Auth.Interfaces;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedicHp.Application.Features.Productivity.Queries.GetDoctorDrafts;

public class GetDoctorDraftsQueryHandler : IRequestHandler<GetDoctorDraftsQuery, List<DoctorDraftDto>>
{
    private readonly IGenericRepository<Consultation> _consultationRepo;
    private readonly ICurrentUserService _currentUserService;

    public GetDoctorDraftsQueryHandler(
        IGenericRepository<Consultation> consultationRepo,
        ICurrentUserService currentUserService)
    {
        _consultationRepo = consultationRepo;
        _currentUserService = currentUserService;
    }

    public async Task<List<DoctorDraftDto>> Handle(GetDoctorDraftsQuery request, CancellationToken cancellationToken)
    {
        var doctorId = _currentUserService.UserId!.Value;

        var drafts = await _consultationRepo.GetQueryable().AsNoTracking()
            .Include(c => c.Patient)
            .Where(c => c.DoctorId == doctorId && !c.IsFinalized)
            .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
            .Select(c => new DoctorDraftDto
            {
                ConsultationId = c.Id,
                PatientId = c.PatientId,
                PatientName = $"{c.Patient.FirstName} {c.Patient.LastName}",
                CreatedAt = c.CreatedAt,
                LastModifiedAt = c.UpdatedAt ?? c.CreatedAt,
                Diagnosis = c.Diagnosis
            })
            .ToListAsync(cancellationToken);

        return drafts;
    }
}
