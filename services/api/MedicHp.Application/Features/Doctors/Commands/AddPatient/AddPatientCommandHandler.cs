using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MedicHp.Application.Features.Auth.DTOs;
using MedicHp.Application.Features.Auth.Interfaces;
using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Clinical;

namespace MedicHp.Application.Features.Doctors.Commands.AddPatient;

public class AddPatientCommandHandler : IRequestHandler<AddPatientCommand, UserDto>
{
    private readonly IAuthService _authService;
    private readonly IGenericRepository<DoctorFavoriteMedicine> _doctorFavoriteMedicineRepository;

    public AddPatientCommandHandler(
        IAuthService authService,
        IGenericRepository<DoctorFavoriteMedicine> doctorFavoriteMedicineRepository) // Dummy to satisfy dependencies if needed, actually we just need authService
    {
        _authService = authService;
    }

    public async Task<UserDto> Handle(AddPatientCommand request, CancellationToken cancellationToken)
    {
        // Use the auth service to register the patient
        var registerDto = new RegisterPatientDto
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Password = request.Password,
            ConfirmPassword = request.ConfirmPassword
        };

        var result = await _authService.RegisterPatientAsync(registerDto);
        
        // In the future, we can add logic to associate this patient with the doctor
        // For example: creating a record in DoctorPatient linking table if it existed.
        
        return result;
    }
}
