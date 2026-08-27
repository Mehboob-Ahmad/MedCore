using MedicHp.Application.Common;
using MedicHp.Application.Features.Doctors.DTOs;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MedicHp.Application.Features.Doctors.Queries.GetAvailableSlots;

public class GetAvailableSlotsQueryHandler : IRequestHandler<GetAvailableSlotsQuery, List<DoctorSlotDto>>
{
    private readonly IGenericRepository<DoctorProfile> _doctorProfileRepository;
    private readonly IGenericRepository<DoctorAvailability> _availabilityRepository;
    private readonly IGenericRepository<DoctorUnavailability> _unavailabilityRepository;
    private readonly IGenericRepository<Appointment> _appointmentRepository;

    public GetAvailableSlotsQueryHandler(
        IGenericRepository<DoctorProfile> doctorProfileRepository,
        IGenericRepository<DoctorAvailability> availabilityRepository,
        IGenericRepository<DoctorUnavailability> unavailabilityRepository,
        IGenericRepository<Appointment> appointmentRepository)
    {
        _doctorProfileRepository = doctorProfileRepository;
        _availabilityRepository = availabilityRepository;
        _unavailabilityRepository = unavailabilityRepository;
        _appointmentRepository = appointmentRepository;
    }

    public async Task<List<DoctorSlotDto>> Handle(GetAvailableSlotsQuery request, CancellationToken cancellationToken)
    {
        var targetDate = request.Date.Date;

        // Prevent past-date queries
        if (targetDate < DateTime.UtcNow.Date)
            return new List<DoctorSlotDto>();

        // Resolve the doctor profile to get SlotDurationMinutes and profile ID
        var doctorProfile = await _doctorProfileRepository.FirstOrDefaultAsync(
            d => d.UserId == request.DoctorId, null, cancellationToken);
        
        if (doctorProfile == null)
            return new List<DoctorSlotDto>();

        var slotDuration = doctorProfile.SlotDurationMinutes;

        // Check if the doctor is unavailable on this date
        var dateOnly = DateOnly.FromDateTime(targetDate);
        var unavailabilities = await _unavailabilityRepository.GetAsync(
            u => u.DoctorProfileId == doctorProfile.Id && u.StartDate <= dateOnly && u.EndDate >= dateOnly,
            null, cancellationToken);

        if (unavailabilities.Any())
            return new List<DoctorSlotDto>(); // Doctor is on leave

        // Fetch configured availability for the day of week
        var dayOfWeek = (short)targetDate.DayOfWeek;
        var schedules = await _availabilityRepository.GetAsync(
            a => a.DoctorProfileId == doctorProfile.Id && a.DayOfWeek == dayOfWeek,
            null, cancellationToken);

        if (!schedules.Any())
            return new List<DoctorSlotDto>(); // Doctor does not work on this day

        // Fetch existing appointments for the target date (excluding cancelled, rejected, and expired reservations)
        var now = DateTime.UtcNow;
        var existingAppointments = await _appointmentRepository.GetAsync(
            a => a.DoctorId == request.DoctorId &&
                 a.ScheduledAt.Date == targetDate &&
                 a.Status != "Cancelled" && a.Status != "Rejected" &&
                 (a.Status != "Reserved" || (a.ExpiresAt != null && a.ExpiresAt > now)),
            null, cancellationToken);

        // Generate slots from each availability window
        var slots = new List<DoctorSlotDto>();

        foreach (var schedule in schedules.OrderBy(s => s.StartTime))
        {
            var windowStart = targetDate.Add(schedule.StartTime);
            var windowEnd = targetDate.Add(schedule.EndTime);
            var currentSlotStart = windowStart;

            while (currentSlotStart.AddMinutes(slotDuration) <= windowEnd)
            {
                var currentSlotEnd = currentSlotStart.AddMinutes(slotDuration);

                // Check for overlap with any booked appointment
                var isBooked = existingAppointments.Any(a =>
                    a.ScheduledAt < currentSlotEnd &&
                    a.ScheduledAt.AddMinutes(a.DurationMinutes) > currentSlotStart);

                // For today, exclude slots that have already passed
                var isPast = targetDate == DateTime.UtcNow.Date && currentSlotStart <= now;

                slots.Add(new DoctorSlotDto
                {
                    SlotId = Guid.NewGuid(),
                    StartTime = currentSlotStart,
                    EndTime = currentSlotEnd,
                    IsAvailable = !isBooked && !isPast
                });

                currentSlotStart = currentSlotEnd;
            }
        }

        return slots;
    }
}
