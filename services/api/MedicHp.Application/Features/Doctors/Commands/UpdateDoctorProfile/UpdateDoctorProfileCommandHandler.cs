using System.Threading;
using System.Threading.Tasks;
using MedicHp.Shared.Exceptions;
using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Clinical;
using MediatR;

namespace MedicHp.Application.Features.Doctors.Commands.UpdateDoctorProfile;

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
        
        profile.ProfessionalType = request.ProfessionalType;
        profile.RegistrationNumber = request.RegistrationNumber;
        profile.RegulatoryBody = request.RegulatoryBody;
        profile.WhatsAppNumber = request.WhatsAppNumber;
        profile.WhatsAppEnabled = request.WhatsAppEnabled;

        // Clear existing qualifications and map new ones
        profile.Qualifications.Clear();
        foreach (var q in request.Qualifications)
        {
            profile.Qualifications.Add(new DoctorQualification
            {
                Degree = q.Degree,
                Institution = q.Institution,
                CompletionYear = q.CompletionYear
            });
        }

        // Clear existing certifications and map new ones
        profile.Certifications.Clear();
        foreach (var c in request.Certifications)
        {
            profile.Certifications.Add(new DoctorCertification
            {
                Name = c.Name,
                IssuingOrganization = c.IssuingOrganization,
                Year = c.Year
            });
        }

        await _doctorProfileRepository.UpdateAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
