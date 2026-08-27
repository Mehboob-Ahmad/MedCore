using System;
using MediatR;

namespace MedicHp.Application.Features.Patients.Commands.SaveFavoriteDoctor;

public class SaveFavoriteDoctorCommand : IRequest<Guid>
{
    public Guid PatientUserId { get; set; }
    public Guid DoctorUserId { get; set; }
}
