using MediatR;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Admin.DTOs;
using MedicHp.Domain.Entities.Core;
using MedicHp.Domain.Entities.Clinical;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace MedicHp.Application.Features.Admin.Queries.GetSystemStats;

public class GetSystemStatsQueryHandler : IRequestHandler<GetSystemStatsQuery, SystemStatsDto>
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<DoctorProfile> _doctorProfileRepository;
    private readonly IGenericRepository<PatientProfile> _patientProfileRepository;

    public GetSystemStatsQueryHandler(
        IGenericRepository<User> userRepository,
        IGenericRepository<DoctorProfile> doctorProfileRepository,
        IGenericRepository<PatientProfile> patientProfileRepository)
    {
        _userRepository = userRepository;
        _doctorProfileRepository = doctorProfileRepository;
        _patientProfileRepository = patientProfileRepository;
    }

    public async Task<SystemStatsDto> Handle(GetSystemStatsQuery request, CancellationToken cancellationToken)
    {
        var allUsers = await _userRepository.GetAllAsync(cancellationToken);
        var totalUsers = allUsers.Count;
        
        // Approximate Monthly Active Users as users created in the last 30 days
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        var monthlyActive = allUsers.Count(u => u.CreatedAt >= thirtyDaysAgo);

        var allDoctors = await _doctorProfileRepository.GetAllAsync(cancellationToken);
        var totalDoctors = allDoctors.Count;

        var allPatients = await _patientProfileRepository.GetAllAsync(cancellationToken);
        var totalPatients = allPatients.Count;

        return new SystemStatsDto
        {
            TotalUsers = totalUsers,
            TotalDoctors = totalDoctors,
            TotalPatients = totalPatients,
            MonthlyActive = monthlyActive
        };
    }
}
