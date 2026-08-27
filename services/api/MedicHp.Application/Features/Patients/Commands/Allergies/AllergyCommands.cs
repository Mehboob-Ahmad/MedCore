using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using MedicHp.Shared.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MedicHp.Application.Features.Patients.Commands.Allergies;

public class AddAllergyCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public string AllergyName { get; set; } = null!;
    public string? Severity { get; set; }
    public string? Notes { get; set; }
}

public class UpdateAllergyCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? AllergyName { get; set; }
    public string? Severity { get; set; }
    public string? Notes { get; set; }
}

public class DeleteAllergyCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}

public class AllergyCommandHandlers : 
    IRequestHandler<AddAllergyCommand, Guid>,
    IRequestHandler<UpdateAllergyCommand, bool>,
    IRequestHandler<DeleteAllergyCommand, bool>
{
    private readonly IGenericRepository<PatientProfile> _patientProfileRepository;
    private readonly IGenericRepository<PatientAllergy> _allergyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AllergyCommandHandlers(
        IGenericRepository<PatientProfile> patientProfileRepository,
        IGenericRepository<PatientAllergy> allergyRepository,
        IUnitOfWork unitOfWork)
    {
        _patientProfileRepository = patientProfileRepository;
        _allergyRepository = allergyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AddAllergyCommand request, CancellationToken cancellationToken)
    {
        var profile = await _patientProfileRepository.FirstOrDefaultAsync(p => p.UserId == request.UserId, null, cancellationToken);
        if (profile == null) throw new NotFoundException(nameof(PatientProfile), request.UserId);

        var allergy = new PatientAllergy
        {
            PatientProfileId = profile.Id,
            AllergyName = request.AllergyName,
            Severity = request.Severity,
            Notes = request.Notes
        };

        await _allergyRepository.AddAsync(allergy, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return allergy.Id;
    }

    public async Task<bool> Handle(UpdateAllergyCommand request, CancellationToken cancellationToken)
    {
        var profile = await _patientProfileRepository.FirstOrDefaultAsync(p => p.UserId == request.UserId, null, cancellationToken);
        if (profile == null) throw new NotFoundException(nameof(PatientProfile), request.UserId);

        var allergy = await _allergyRepository.GetByIdAsync(request.Id, cancellationToken);
        if (allergy == null || allergy.PatientProfileId != profile.Id)
            throw new NotFoundException(nameof(PatientAllergy), request.Id);

        if (request.AllergyName != null) allergy.AllergyName = request.AllergyName;
        if (request.Severity != null) allergy.Severity = request.Severity;
        if (request.Notes != null) allergy.Notes = request.Notes;

        await _allergyRepository.UpdateAsync(allergy, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> Handle(DeleteAllergyCommand request, CancellationToken cancellationToken)
    {
        var profile = await _patientProfileRepository.FirstOrDefaultAsync(p => p.UserId == request.UserId, null, cancellationToken);
        if (profile == null) throw new NotFoundException(nameof(PatientProfile), request.UserId);

        var allergy = await _allergyRepository.GetByIdAsync(request.Id, cancellationToken);
        if (allergy == null || allergy.PatientProfileId != profile.Id)
            throw new NotFoundException(nameof(PatientAllergy), request.Id);

        await _allergyRepository.DeleteAsync(allergy, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
