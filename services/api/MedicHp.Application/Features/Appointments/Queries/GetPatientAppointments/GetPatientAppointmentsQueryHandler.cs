using MedicHp.Application.Common;
using MedicHp.Application.Features.Appointments.DTOs;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MedicHp.Application.Features.Appointments.Queries.GetPatientAppointments;

public class GetPatientAppointmentsQueryHandler : IRequestHandler<GetPatientAppointmentsQuery, List<AppointmentDto>>
{
    private readonly IGenericRepository<Appointment> _appointmentRepository;

    public GetPatientAppointmentsQueryHandler(IGenericRepository<Appointment> appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<List<AppointmentDto>> Handle(GetPatientAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var query = _appointmentRepository.GetQueryable().AsNoTracking()
            .Include(a => a.Doctor)
                .ThenInclude(d => d.DoctorProfile)
                    .ThenInclude(dp => dp!.Specializations)
                        .ThenInclude(s => s.Specialization)
            .Include(a => a.Doctor)
                .ThenInclude(d => d.DoctorProfile)
                    .ThenInclude(dp => dp!.City)
            .Include(a => a.Doctor)
                .ThenInclude(d => d.DoctorProfile)
                    .ThenInclude(dp => dp!.PaymentMethods)
            .Include(a => a.Doctor)
                .ThenInclude(d => d.ProfilePhotoFile)
            .AsNoTracking()
            .Where(a => a.PatientId == request.UserId);

        // Apply preset filters
        if (!string.IsNullOrWhiteSpace(request.Filter))
        {
            query = request.Filter.ToLower() switch
            {
                "upcoming" => query.Where(a => a.ScheduledAt >= now && a.Status != "Cancelled" && a.Status != "Rejected"),
                "past" => query.Where(a => a.ScheduledAt < now || a.Status == "Completed"),
                "pending" => query.Where(a => a.Status == "Pending"),
                "cancelled" => query.Where(a => a.Status == "Cancelled"),
                "rejected" => query.Where(a => a.Status == "Rejected"),
                _ => query
            };
        }

        // Apply specific status filter
        if (!string.IsNullOrWhiteSpace(request.Status))
            query = query.Where(a => a.Status == request.Status);

        // Apply date range filter
        if (request.DateFrom.HasValue)
            query = query.Where(a => a.ScheduledAt >= request.DateFrom.Value);
        if (request.DateTo.HasValue)
            query = query.Where(a => a.ScheduledAt <= request.DateTo.Value);

        // Apply doctor filter
        if (request.DoctorId.HasValue)
            query = query.Where(a => a.DoctorId == request.DoctorId.Value);

        var appointments = await query
            .OrderByDescending(a => a.ScheduledAt)
            .ToListAsync(cancellationToken);

        return appointments.Select(a => new AppointmentDto
        {
            Id = a.Id,
            DoctorId = a.DoctorId,
            DoctorName = $"Dr. {a.Doctor?.FirstName} {a.Doctor?.LastName}",
            DoctorProfilePhotoUrl = a.Doctor?.ProfilePhotoFile?.StoragePath,
            Specialty = a.Doctor?.DoctorProfile?.Specializations.FirstOrDefault()?.Specialization?.Name,
            ClinicName = a.Doctor?.DoctorProfile?.ClinicName,
            ClinicAddress = a.Doctor?.DoctorProfile?.Address,
            PatientId = a.PatientId,
            ScheduledAt = a.ScheduledAt,
            StartTime = a.ScheduledAt.ToString("HH:mm"),
            EndTime = a.ScheduledAt.AddMinutes(a.DurationMinutes).ToString("HH:mm"),
            DurationMinutes = a.DurationMinutes,
            Status = a.Status,
            StatusColor = AppointmentStatusColors.GetColor(a.Status),
            BookingNote = a.BookingNote,
            CancellationReason = a.CancellationReason,
            DoctorNotes = a.DoctorNotes,
            ConsultationFee = a.Doctor?.DoctorProfile?.ConsultationFee,
            SuggestedNewTime = a.SuggestedNewTime,
            PaymentStatus = a.PaymentStatus,
            PaymentMethods = a.Doctor?.DoctorProfile?.PaymentMethods?.Where(pm => pm.IsActive).Select(pm => new MedicHp.Application.Features.Doctors.DTOs.DoctorPaymentMethodDto
            {
                Id = pm.Id,
                PaymentMethodType = pm.PaymentMethodType,
                PaymentProvider = pm.PaymentProvider,
                AccountTitle = pm.AccountTitle,
                AccountNumber = pm.AccountNumber,
                MaskedAccountNumber = !string.IsNullOrEmpty(pm.AccountNumber) && pm.AccountNumber.Length >= 4
                    ? new string('*', pm.AccountNumber.Length - 4) + pm.AccountNumber.Substring(pm.AccountNumber.Length - 4)
                    : pm.AccountNumber,
                IBAN = pm.IBAN,
                MaskedIBAN = !string.IsNullOrEmpty(pm.IBAN) && pm.IBAN.Length >= 4
                    ? new string('*', pm.IBAN.Length - 4) + pm.IBAN.Substring(pm.IBAN.Length - 4)
                    : pm.IBAN,
                IsActive = pm.IsActive
            }).ToList(),
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        }).ToList();
    }
}
