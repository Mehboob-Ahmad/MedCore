using MediatR;
using System;

namespace MedicHp.Application.Features.Patients.Commands.UpdatePatientProfile;

public class UpdatePatientProfileCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? BloodType { get; set; }
    public Guid? CityId { get; set; }
    public string? Address { get; set; }
    public bool? DataSharingConsent { get; set; }
}
