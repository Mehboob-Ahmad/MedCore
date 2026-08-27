using System;
using MediatR;

namespace MedicHp.Application.Features.Patients.Commands.RemoveFavoriteDoctor;

public class RemoveFavoriteDoctorCommand : IRequest<bool>
{
    public Guid PatientUserId { get; set; }
    public Guid DoctorUserId { get; set; }
}
