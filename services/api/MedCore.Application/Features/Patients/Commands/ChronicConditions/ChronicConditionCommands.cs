using MedCore.Application.Common;
using MedCore.Domain.Entities.Clinical;
using MediatR;
using MedCore.Shared.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MedCore.Application.Features.Patients.Commands.ChronicConditions;

public class AddChronicConditionCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public string ConditionName { get; set; } = null!;
    public DateTime? DiagnosedDate { get; set; }
    public string? Notes { get; set; }
}

public class UpdateChronicConditionCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? ConditionName { get; set; }
    public DateTime? DiagnosedDate { get; set; }
    public string? Notes { get; set; }
}

public class DeleteChronicConditionCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}

public class ChronicConditionCommandHandlers : 
    IRequestHandler<AddChronicConditionCommand, Guid>,
    IRequestHandler<UpdateChronicConditionCommand, bool>,
    IRequestHandler<DeleteChronicConditionCommand, bool>
{
    private readonly IGenericRepository<PatientProfile> _patientProfileRepository;
    private readonly IGenericRepository<PatientChronicCondition> _conditionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChronicConditionCommandHandlers(
        IGenericRepository<PatientProfile> patientProfileRepository,
        IGenericRepository<PatientChronicCondition> conditionRepository,
        IUnitOfWork unitOfWork)
    {
        _patientProfileRepository = patientProfileRepository;
        _conditionRepository = conditionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AddChronicConditionCommand request, CancellationToken cancellationToken)
    {
        var profile = await _patientProfileRepository.FirstOrDefaultAsync(p => p.UserId == request.UserId, null, cancellationToken);
        if (profile == null) throw new NotFoundException(nameof(PatientProfile), request.UserId);

        var condition = new PatientChronicCondition
        {
            PatientProfileId = profile.Id,
            ConditionName = request.ConditionName,
            DiagnosedDate = request.DiagnosedDate.HasValue ? DateOnly.FromDateTime(request.DiagnosedDate.Value) : null,
            Notes = request.Notes
        };

        await _conditionRepository.AddAsync(condition, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return condition.Id;
    }

    public async Task<bool> Handle(UpdateChronicConditionCommand request, CancellationToken cancellationToken)
    {
        var profile = await _patientProfileRepository.FirstOrDefaultAsync(p => p.UserId == request.UserId, null, cancellationToken);
        if (profile == null) throw new NotFoundException(nameof(PatientProfile), request.UserId);

        var condition = await _conditionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (condition == null || condition.PatientProfileId != profile.Id)
            throw new NotFoundException(nameof(PatientChronicCondition), request.Id);

        if (request.ConditionName != null) condition.ConditionName = request.ConditionName;
        if (request.DiagnosedDate.HasValue) condition.DiagnosedDate = DateOnly.FromDateTime(request.DiagnosedDate.Value);
        if (request.Notes != null) condition.Notes = request.Notes;

        await _conditionRepository.UpdateAsync(condition, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> Handle(DeleteChronicConditionCommand request, CancellationToken cancellationToken)
    {
        var profile = await _patientProfileRepository.FirstOrDefaultAsync(p => p.UserId == request.UserId, null, cancellationToken);
        if (profile == null) throw new NotFoundException(nameof(PatientProfile), request.UserId);

        var condition = await _conditionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (condition == null || condition.PatientProfileId != profile.Id)
            throw new NotFoundException(nameof(PatientChronicCondition), request.Id);

        await _conditionRepository.DeleteAsync(condition, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
