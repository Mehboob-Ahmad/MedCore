using System;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Admin;
using MediatR;

namespace MedicHp.Application.Features.Public.Commands.SubmitDemoRequest;

public class SubmitDemoRequestCommandHandler : IRequestHandler<SubmitDemoRequestCommand, Guid>
{
    private readonly IGenericRepository<DemoRequest> _demoRequestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitDemoRequestCommandHandler(IGenericRepository<DemoRequest> demoRequestRepository, IUnitOfWork unitOfWork)
    {
        _demoRequestRepository = demoRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(SubmitDemoRequestCommand request, CancellationToken cancellationToken)
    {
        var demoRequest = new DemoRequest
        {
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Specialization = request.Specialization,
            City = request.City,
            ClinicOrHospital = request.ClinicOrHospital,
            YearsOfExperience = request.YearsOfExperience,
            ProfessionalQualification = request.ProfessionalQualification,
            AdditionalInformation = request.AdditionalInformation,
            DegreeImageUrl = request.DegreeImageUrl,
            LicenseImageUrl = request.LicenseImageUrl,
            Status = DemoRequestStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _demoRequestRepository.AddAsync(demoRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // NOTE: In the future, we will fire an email to the super admin or the doctor via IEmailService here.

        return demoRequest.Id;
    }
}
