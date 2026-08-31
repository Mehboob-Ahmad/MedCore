using System;
using MediatR;
using MedicHp.Application.Features.Auth.DTOs;

namespace MedicHp.Application.Features.Doctors.Commands.AddPatient;

public class AddPatientCommand : IRequest<UserDto>
{
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
    
    // Doctor adding the patient
    public Guid DoctorId { get; set; }
}
