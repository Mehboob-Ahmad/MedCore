using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Shared.Exceptions;
using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedicHp.Application.Features.Doctors.Commands.ConfigureAvailability;

public class ConfigureAvailabilityCommandHandler : IRequestHandler<ConfigureAvailabilityCommand, bool>
{
    private readonly IGenericRepository<DoctorProfile> _doctorProfileRepository;
    private readonly IGenericRepository<DoctorAvailability> _availabilityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfigureAvailabilityCommandHandler(
        IGenericRepository<DoctorProfile> doctorProfileRepository,
        IGenericRepository<DoctorAvailability> availabilityRepository,
        IUnitOfWork unitOfWork)
    {
        _doctorProfileRepository = doctorProfileRepository;
        _availabilityRepository = availabilityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ConfigureAvailabilityCommand request, CancellationToken cancellationToken)
    {
        var doctorProfile = await _doctorProfileRepository.FirstOrDefaultAsync(
            d => d.UserId == request.UserId,
            include: q => q.Include(d => d.Availabilities),
            cancellationToken: cancellationToken);

        if (doctorProfile == null)
        {
            throw new NotFoundException(nameof(DoctorProfile), request.UserId);
        }

        foreach (var day in request.Days)
        {
            var existingForDay = doctorProfile.Availabilities.Where(a => a.DayOfWeek == (short)day.DayOfWeek).ToList();
            
            foreach (var existing in existingForDay)
            {
                await _availabilityRepository.DeleteAsync(existing, cancellationToken);
            }

            if (day.IsAvailable)
            {
                if (TimeSpan.TryParse(day.StartTime, out var startTime) && TimeSpan.TryParse(day.EndTime, out var endTime))
                {
                    var newAvailability = new DoctorAvailability
                    {
                        DoctorProfileId = doctorProfile.Id,
                        DayOfWeek = (short)day.DayOfWeek,
                        StartTime = startTime,
                        EndTime = endTime
                    };

                    await _availabilityRepository.AddAsync(newAvailability, cancellationToken);
                }
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
