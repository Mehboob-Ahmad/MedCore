using System.Threading;
using System.Threading.Tasks;
using MedCore.Shared.Exceptions;
using MedCore.Application.Common;
using MedCore.Domain.Entities.Clinical;
using MediatR;

namespace MedCore.Application.Features.Doctors.Commands.UpdateDoctorProfile;

public class UpdateDoctorProfileCommandHandler : IRequestHandler<UpdateDoctorProfileCommand, bool>
{
    private readonly IGenericRepository<DoctorProfile> _doctorProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDoctorProfileCommandHandler(
        IGenericRepository<DoctorProfile> doctorProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _doctorProfileRepository = doctorProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateDoctorProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _doctorProfileRepository.FirstOrDefaultAsync(
            d => d.UserId == request.UserId,
            cancellationToken: cancellationToken);

        if (profile == null)
        {
            throw new NotFoundException(nameof(DoctorProfile), request.UserId);
        }

        profile.Bio = request.Bio;
        profile.ConsultationFee = request.ConsultationFee;
        profile.YearsOfExperience = request.ExperienceYears;

        await _doctorProfileRepository.UpdateAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
