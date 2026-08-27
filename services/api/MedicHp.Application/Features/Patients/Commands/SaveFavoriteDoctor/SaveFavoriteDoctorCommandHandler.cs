using System;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedicHp.Application.Features.Patients.Commands.SaveFavoriteDoctor;

public class SaveFavoriteDoctorCommandHandler : IRequestHandler<SaveFavoriteDoctorCommand, Guid>
{
    private readonly IGenericRepository<PatientFavoriteDoctor> _favoriteRepository;
    private readonly IGenericRepository<PatientProfile> _patientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SaveFavoriteDoctorCommandHandler(
        IGenericRepository<PatientFavoriteDoctor> favoriteRepository,
        IGenericRepository<PatientProfile> patientRepository,
        IUnitOfWork unitOfWork)
    {
        _favoriteRepository = favoriteRepository;
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(SaveFavoriteDoctorCommand request, CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetQueryable()
            .FirstOrDefaultAsync(p => p.UserId == request.PatientUserId, cancellationToken);
            
        if (patient == null) throw new Exception("Patient not found.");

        var existingFavorite = await _favoriteRepository.GetQueryable()
            .FirstOrDefaultAsync(f => f.PatientId == patient.Id && f.DoctorId == request.DoctorUserId, cancellationToken);
            
        if (existingFavorite != null)
        {
            return existingFavorite.Id;
        }
        
        var favorite = new PatientFavoriteDoctor
        {
            PatientId = patient.Id,
            DoctorId = request.DoctorUserId
        };
        
        await _favoriteRepository.AddAsync(favorite, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return favorite.Id;
    }
}
