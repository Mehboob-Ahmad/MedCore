using System;
using System.Collections.Generic;
using MedicHp.Application.Features.Doctors.DTOs;
using MediatR;

namespace MedicHp.Application.Features.Doctors.Queries.GetDoctorPaymentMethods;

public class GetDoctorPaymentMethodsQuery : IRequest<List<DoctorPaymentMethodDto>>
{
    public Guid DoctorId { get; set; }

    public GetDoctorPaymentMethodsQuery(Guid doctorId)
    {
        DoctorId = doctorId;
    }
}
