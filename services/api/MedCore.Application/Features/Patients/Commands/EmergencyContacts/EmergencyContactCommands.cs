using MedCore.Application.Common;
using MedCore.Domain.Entities.Clinical;
using MediatR;
using MedCore.Shared.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MedCore.Application.Features.Patients.Commands.EmergencyContacts;

public class AddEmergencyContactCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string Relationship { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public bool IsPrimary { get; set; }
}

public class UpdateEmergencyContactCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? FullName { get; set; }
    public string? Relationship { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? IsPrimary { get; set; }
}

public class DeleteEmergencyContactCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}

public class EmergencyContactCommandHandlers : 
    IRequestHandler<AddEmergencyContactCommand, Guid>,
    IRequestHandler<UpdateEmergencyContactCommand, bool>,
    IRequestHandler<DeleteEmergencyContactCommand, bool>
{
    private readonly IGenericRepository<PatientProfile> _patientProfileRepository;
    private readonly IGenericRepository<EmergencyContact> _emergencyContactRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EmergencyContactCommandHandlers(
        IGenericRepository<PatientProfile> patientProfileRepository,
        IGenericRepository<EmergencyContact> emergencyContactRepository,
        IUnitOfWork unitOfWork)
    {
        _patientProfileRepository = patientProfileRepository;
        _emergencyContactRepository = emergencyContactRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AddEmergencyContactCommand request, CancellationToken cancellationToken)
    {
        var profile = await _patientProfileRepository.FirstOrDefaultAsync(p => p.UserId == request.UserId, null, cancellationToken);
        if (profile == null) throw new NotFoundException(nameof(PatientProfile), request.UserId);

        var contact = new EmergencyContact
        {
            PatientProfileId = profile.Id,
            FullName = request.FullName,
            Relationship = request.Relationship,
            PhoneNumber = request.PhoneNumber,
            IsPrimary = request.IsPrimary
        };

        await _emergencyContactRepository.AddAsync(contact, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return contact.Id;
    }

    public async Task<bool> Handle(UpdateEmergencyContactCommand request, CancellationToken cancellationToken)
    {
        var profile = await _patientProfileRepository.FirstOrDefaultAsync(p => p.UserId == request.UserId, null, cancellationToken);
        if (profile == null) throw new NotFoundException(nameof(PatientProfile), request.UserId);

        var contact = await _emergencyContactRepository.GetByIdAsync(request.Id, cancellationToken);
        if (contact == null || contact.PatientProfileId != profile.Id)
            throw new NotFoundException(nameof(EmergencyContact), request.Id);

        if (request.FullName != null) contact.FullName = request.FullName;
        if (request.Relationship != null) contact.Relationship = request.Relationship;
        if (request.PhoneNumber != null) contact.PhoneNumber = request.PhoneNumber;
        if (request.IsPrimary.HasValue) contact.IsPrimary = request.IsPrimary.Value;

        await _emergencyContactRepository.UpdateAsync(contact, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> Handle(DeleteEmergencyContactCommand request, CancellationToken cancellationToken)
    {
        var profile = await _patientProfileRepository.FirstOrDefaultAsync(p => p.UserId == request.UserId, null, cancellationToken);
        if (profile == null) throw new NotFoundException(nameof(PatientProfile), request.UserId);

        var contact = await _emergencyContactRepository.GetByIdAsync(request.Id, cancellationToken);
        if (contact == null || contact.PatientProfileId != profile.Id)
            throw new NotFoundException(nameof(EmergencyContact), request.Id);

        await _emergencyContactRepository.DeleteAsync(contact, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
