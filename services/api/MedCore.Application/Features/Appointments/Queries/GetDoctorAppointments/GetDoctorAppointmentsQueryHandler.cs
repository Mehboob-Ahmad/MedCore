using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedCore.Application.Common;
using MedCore.Application.Features.Appointments.DTOs;
using MedCore.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Application.Features.Appointments.Queries.GetDoctorAppointments;

public class GetDoctorAppointmentsQueryHandler : IRequestHandler<GetDoctorAppointmentsQuery, List<AppointmentDto>>
{
    private readonly IGenericRepository<Appointment> _appointmentRepository;

    public GetDoctorAppointmentsQueryHandler(IGenericRepository<Appointment> appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<List<AppointmentDto>> Handle(GetDoctorAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var query = _appointmentRepository.GetQueryable().AsNoTracking()
            .Include(a => a.Patient)
                .ThenInclude(p => p.ProfilePhotoFile)
            .AsNoTracking()
            .Where(a => a.DoctorId == request.DoctorId);

        // Apply preset filters
        if (!string.IsNullOrWhiteSpace(request.Filter))
        {
            query = request.Filter.ToLower() switch
            {
                "today" => query.Where(a => a.ScheduledAt >= today && a.ScheduledAt < tomorrow),
                "upcoming" => query.Where(a => a.ScheduledAt >= tomorrow && a.Status != "Cancelled" && a.Status != "Rejected"),
                "pending" => query.Where(a => a.Status == "Pending"),
                "completed" => query.Where(a => a.Status == "Completed"),
                "cancelled" => query.Where(a => a.Status == "Cancelled"),
                "missed" => query.Where(a => a.Status == "NoShow"),
                "rescheduled" => query.Where(a => a.Status == "Rescheduled"),
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

        // Apply patient filter
        if (request.PatientId.HasValue)
            query = query.Where(a => a.PatientId == request.PatientId.Value);

        // Apply search (patient name)
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.ToLower();
            query = query.Where(a =>
                a.Patient.FirstName.ToLower().Contains(search) ||
                a.Patient.LastName.ToLower().Contains(search));
        }

        var appointments = await query
            .OrderBy(a => a.ScheduledAt)
            .ToListAsync(cancellationToken);

        return appointments.Select(a => new AppointmentDto
        {
            Id = a.Id,
            DoctorId = a.DoctorId,
            PatientId = a.PatientId,
            PatientName = $"{a.Patient?.FirstName} {a.Patient?.LastName}",
            ScheduledAt = a.ScheduledAt,
            StartTime = a.ScheduledAt.ToString("HH:mm"),
            EndTime = a.ScheduledAt.AddMinutes(a.DurationMinutes).ToString("HH:mm"),
            DurationMinutes = a.DurationMinutes,
            Status = a.Status,
            StatusColor = AppointmentStatusColors.GetColor(a.Status),
            BookingNote = a.BookingNote,
            CancellationReason = a.CancellationReason,
            DoctorNotes = a.DoctorNotes,
            SuggestedNewTime = a.SuggestedNewTime,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        }).ToList();
    }
}
