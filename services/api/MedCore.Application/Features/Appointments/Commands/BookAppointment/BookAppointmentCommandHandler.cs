using MedCore.Application.Common;
using MedCore.Domain.Entities.Clinical;
using MedCore.Domain.Entities.Core;
using MediatR;
using MedCore.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MedCore.Application.Features.Appointments.Commands.BookAppointment;

public class BookAppointmentCommandHandler : IRequestHandler<BookAppointmentCommand, Guid>
{
    private readonly IGenericRepository<PatientProfile> _patientProfileRepository;
    private readonly IGenericRepository<DoctorProfile> _doctorProfileRepository;
    private readonly IGenericRepository<DoctorAvailability> _availabilityRepository;
    private readonly IGenericRepository<DoctorUnavailability> _unavailabilityRepository;
    private readonly IGenericRepository<Appointment> _appointmentRepository;
    private readonly IGenericRepository<AppointmentStatusHistory> _statusHistoryRepository;
    private readonly IGenericRepository<Notification> _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BookAppointmentCommandHandler(
        IGenericRepository<PatientProfile> patientProfileRepository,
        IGenericRepository<DoctorProfile> doctorProfileRepository,
        IGenericRepository<DoctorAvailability> availabilityRepository,
        IGenericRepository<DoctorUnavailability> unavailabilityRepository,
        IGenericRepository<Appointment> appointmentRepository,
        IGenericRepository<AppointmentStatusHistory> statusHistoryRepository,
        IGenericRepository<Notification> notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _patientProfileRepository = patientProfileRepository;
        _doctorProfileRepository = doctorProfileRepository;
        _availabilityRepository = availabilityRepository;
        _unavailabilityRepository = unavailabilityRepository;
        _appointmentRepository = appointmentRepository;
        _statusHistoryRepository = statusHistoryRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate patient exists
        var patient = await _patientProfileRepository.FirstOrDefaultAsync(
            p => p.UserId == request.UserId, null, cancellationToken);
        if (patient == null)
            throw new NotFoundException(nameof(PatientProfile), request.UserId);

        // 2. Validate doctor exists and is verified
        var doctor = await _doctorProfileRepository.FirstOrDefaultAsync(
            d => d.UserId == request.DoctorId,
            q => q.Include(d => d.User).Include(d => d.Specializations).ThenInclude(s => s.Specialization),
            cancellationToken);
        if (doctor == null || doctor.VerificationStatus != "Verified")
            throw new NotFoundException(nameof(DoctorProfile), request.DoctorId);

        // 3. Parse time and build ScheduledAt
        var startTime = TimeOnly.Parse(request.StartTime);
        var scheduledAt = request.ScheduledDate.Date.Add(startTime.ToTimeSpan());
        var durationMinutes = doctor.SlotDurationMinutes;
        var endTime = scheduledAt.AddMinutes(durationMinutes);

        // 4. Prevent past-date booking
        if (scheduledAt <= DateTime.UtcNow)
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure("ScheduledDate", "Cannot book appointments in the past.")
            });

        // 5. Check doctor unavailability (leave days)
        var dateOnly = DateOnly.FromDateTime(request.ScheduledDate);
        var unavailabilities = await _unavailabilityRepository.GetAsync(
            u => u.DoctorProfileId == doctor.Id && u.StartDate <= dateOnly && u.EndDate >= dateOnly,
            null, cancellationToken);
        if (unavailabilities.Any())
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure("ScheduledDate", "Doctor is unavailable on the selected date.")
            });

        // 6. Check doctor availability (working hours for the day of week)
        var dayOfWeek = (short)request.ScheduledDate.DayOfWeek;
        var schedules = await _availabilityRepository.GetAsync(
            a => a.DoctorProfileId == doctor.Id && a.DayOfWeek == dayOfWeek,
            null, cancellationToken);
        if (!schedules.Any())
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure("ScheduledDate", "Doctor does not work on the selected day.")
            });

        var slotStartTimeSpan = startTime.ToTimeSpan();
        var slotEndTimeSpan = slotStartTimeSpan.Add(TimeSpan.FromMinutes(durationMinutes));
        var fitsSchedule = schedules.Any(s => s.StartTime <= slotStartTimeSpan && s.EndTime >= slotEndTimeSpan);
        if (!fitsSchedule)
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure("StartTime", "Selected time is outside the doctor's working hours.")
            });

        // 7. Prevent double-booking (check for overlapping non-cancelled, non-expired reserved appointments)
        var now = DateTime.UtcNow;
        var conflictingAppointments = await _appointmentRepository.GetAsync(
            a => a.DoctorId == request.DoctorId &&
                 a.ScheduledAt < endTime &&
                 a.ScheduledAt.AddMinutes(a.DurationMinutes) > scheduledAt &&
                 a.Status != "Cancelled" && a.Status != "Rejected" &&
                 (a.Status != "Reserved" || (a.ExpiresAt != null && a.ExpiresAt > now)),
            null, cancellationToken);
        if (conflictingAppointments.Any())
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure("StartTime", "This time slot is already booked.")
            });

        // 8. Create the appointment as Pending
        var appointment = new Appointment
        {
            PatientId = request.UserId,
            DoctorId = request.DoctorId,
            ScheduledAt = scheduledAt,
            DurationMinutes = durationMinutes,
            BookingNote = request.BookingNote,
            Status = "Pending"
        };

        await _appointmentRepository.AddAsync(appointment, cancellationToken);

        // 9. Record status history
        var statusHistory = new AppointmentStatusHistory
        {
            AppointmentId = appointment.Id,
            FromStatus = "New",
            ToStatus = "Pending",
            ChangedByUserId = request.UserId,
            Reason = "Appointment booked by patient."
        };
        await _statusHistoryRepository.AddAsync(statusHistory, cancellationToken);

        // 10. Create notification for doctor
        var notification = new Notification
        {
            UserId = request.DoctorId,
            Type = "AppointmentRequested",
            Title = "New Appointment Request",
            Body = $"You have a new appointment request for {scheduledAt:MMM dd, yyyy} at {scheduledAt:HH:mm}.",
            ReferenceType = "Appointment",
            ReferenceId = appointment.Id,
            SentAt = DateTime.UtcNow
        };
        await _notificationRepository.AddAsync(notification, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return appointment.Id;
    }
}
