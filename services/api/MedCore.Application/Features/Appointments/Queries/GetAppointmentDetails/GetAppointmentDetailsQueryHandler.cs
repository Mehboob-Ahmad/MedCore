using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedCore.Application.Common;
using MedCore.Application.Features.Appointments.DTOs;
using MedCore.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Application.Features.Appointments.Queries.GetAppointmentDetails;

public class GetAppointmentDetailsQueryHandler : IRequestHandler<GetAppointmentDetailsQuery, AppointmentDetailDto?>
{
    private readonly IGenericRepository<Appointment> _appointmentRepository;

    public GetAppointmentDetailsQueryHandler(IGenericRepository<Appointment> appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<AppointmentDetailDto?> Handle(GetAppointmentDetailsQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetQueryable().AsNoTracking()
            .Include(a => a.Patient)
                .ThenInclude(p => p.ProfilePhotoFile)
            .Include(a => a.Doctor)
                .ThenInclude(d => d.DoctorProfile)
                    .ThenInclude(dp => dp!.Specializations)
                        .ThenInclude(s => s.Specialization)
            .Include(a => a.Doctor)
                .ThenInclude(d => d.DoctorProfile)
                    .ThenInclude(dp => dp!.City)
            .Include(a => a.Doctor)
                .ThenInclude(d => d.ProfilePhotoFile)
            .Include(a => a.StatusHistory.OrderBy(sh => sh.CreatedAt))
                .ThenInclude(sh => sh.ChangedByUser)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);

        if (appointment == null)
            return null;

        // Authorization: only the patient or doctor involved can view details
        if (appointment.PatientId != request.UserId && appointment.DoctorId != request.UserId)
            return null;

        return new AppointmentDetailDto
        {
            Id = appointment.Id,
            DoctorId = appointment.DoctorId,
            DoctorName = $"Dr. {appointment.Doctor?.FirstName} {appointment.Doctor?.LastName}",
            DoctorProfilePhotoUrl = appointment.Doctor?.ProfilePhotoFile?.StoragePath,
            Specialty = appointment.Doctor?.DoctorProfile?.Specializations.FirstOrDefault()?.Specialization?.Name,
            ClinicName = appointment.Doctor?.DoctorProfile?.ClinicName,
            ClinicAddress = appointment.Doctor?.DoctorProfile?.Address,
            PatientId = appointment.PatientId,
            PatientName = $"{appointment.Patient?.FirstName} {appointment.Patient?.LastName}",
            ScheduledAt = appointment.ScheduledAt,
            StartTime = appointment.ScheduledAt.ToString("HH:mm"),
            EndTime = appointment.ScheduledAt.AddMinutes(appointment.DurationMinutes).ToString("HH:mm"),
            DurationMinutes = appointment.DurationMinutes,
            Status = appointment.Status,
            StatusColor = AppointmentStatusColors.GetColor(appointment.Status),
            BookingNote = appointment.BookingNote,
            CancellationReason = appointment.CancellationReason,
            DoctorNotes = appointment.DoctorNotes,
            ConsultationFee = appointment.Doctor?.DoctorProfile?.ConsultationFee,
            SuggestedNewTime = appointment.SuggestedNewTime,
            CreatedAt = appointment.CreatedAt,
            UpdatedAt = appointment.UpdatedAt,
            StatusTimeline = appointment.StatusHistory.Select(sh => new AppointmentStatusHistoryDto
            {
                FromStatus = sh.FromStatus,
                ToStatus = sh.ToStatus,
                ChangedByName = sh.ChangedByUser != null
                    ? $"{sh.ChangedByUser.FirstName} {sh.ChangedByUser.LastName}"
                    : null,
                Reason = sh.Reason,
                ChangedAt = sh.CreatedAt
            }).ToList()
        };
    }
}
