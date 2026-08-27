using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using MedicHp.Shared.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MedicHp.Application.Features.Patients.Commands.Medications;

public class AddMedicationCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public string MedicationName { get; set; } = null!;
    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
}

public class UpdateMedicationCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? MedicationName { get; set; }
    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
}

public class DeleteMedicationCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}

public class MedicationCommandHandlers : 
    IRequestHandler<AddMedicationCommand, Guid>,
    IRequestHandler<UpdateMedicationCommand, bool>,
    IRequestHandler<DeleteMedicationCommand, bool>
{
    private readonly IGenericRepository<PatientProfile> _patientProfileRepository;
    private readonly IGenericRepository<PatientMedication> _medicationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MedicationCommandHandlers(
        IGenericRepository<PatientProfile> patientProfileRepository,
        IGenericRepository<PatientMedication> medicationRepository,
        IUnitOfWork unitOfWork)
    {
        _patientProfileRepository = patientProfileRepository;
        _medicationRepository = medicationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AddMedicationCommand request, CancellationToken cancellationToken)
    {
        var profile = await _patientProfileRepository.FirstOrDefaultAsync(p => p.UserId == request.UserId, null, cancellationToken);
        if (profile == null) throw new NotFoundException(nameof(PatientProfile), request.UserId);

        var medication = new PatientMedication
        {
            PatientProfileId = profile.Id,
            MedicationName = request.MedicationName,
            Dosage = request.Dosage,
            Frequency = request.Frequency
        };

        await _medicationRepository.AddAsync(medication, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return medication.Id;
    }

    public async Task<bool> Handle(UpdateMedicationCommand request, CancellationToken cancellationToken)
    {
        var profile = await _patientProfileRepository.FirstOrDefaultAsync(p => p.UserId == request.UserId, null, cancellationToken);
        if (profile == null) throw new NotFoundException(nameof(PatientProfile), request.UserId);

        var medication = await _medicationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (medication == null || medication.PatientProfileId != profile.Id)
            throw new NotFoundException(nameof(PatientMedication), request.Id);

        if (request.MedicationName != null) medication.MedicationName = request.MedicationName;
        if (request.Dosage != null) medication.Dosage = request.Dosage;
        if (request.Frequency != null) medication.Frequency = request.Frequency;

        await _medicationRepository.UpdateAsync(medication, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> Handle(DeleteMedicationCommand request, CancellationToken cancellationToken)
    {
        var profile = await _patientProfileRepository.FirstOrDefaultAsync(p => p.UserId == request.UserId, null, cancellationToken);
        if (profile == null) throw new NotFoundException(nameof(PatientProfile), request.UserId);

        var medication = await _medicationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (medication == null || medication.PatientProfileId != profile.Id)
            throw new NotFoundException(nameof(PatientMedication), request.Id);

        await _medicationRepository.DeleteAsync(medication, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
