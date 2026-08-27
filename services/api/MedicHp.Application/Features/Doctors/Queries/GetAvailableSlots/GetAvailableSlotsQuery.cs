using MedicHp.Application.Features.Doctors.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace MedicHp.Application.Features.Doctors.Queries.GetAvailableSlots;

public class GetAvailableSlotsQuery : IRequest<List<DoctorSlotDto>>
{
    public Guid DoctorId { get; set; }
    public DateTime Date { get; set; }

    public GetAvailableSlotsQuery(Guid doctorId, DateTime date)
    {
        DoctorId = doctorId;
        Date = date;
    }
}
