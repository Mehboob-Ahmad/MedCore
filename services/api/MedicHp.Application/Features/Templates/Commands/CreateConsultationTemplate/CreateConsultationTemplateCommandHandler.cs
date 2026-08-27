using System;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Auth.Interfaces;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedicHp.Application.Features.Templates.Commands.CreateConsultationTemplate;

public class CreateConsultationTemplateCommandHandler : IRequestHandler<CreateConsultationTemplateCommand, Guid>
{
    private readonly IGenericRepository<ConsultationTemplate> _repository;
    private readonly ICurrentUserService _currentUserService;

    public CreateConsultationTemplateCommandHandler(
        IGenericRepository<ConsultationTemplate> repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateConsultationTemplateCommand request, CancellationToken cancellationToken)
    {
        var doctorId = _currentUserService.UserId!.Value;

        var exists = await _repository.GetQueryable()
            .AnyAsync(t => t.DoctorId == doctorId && t.TemplateName == request.TemplateName, cancellationToken);
            
        if (exists)
        {
            throw new Exception($"Template with name '{request.TemplateName}' already exists.");
        }

        var template = new ConsultationTemplate
        {
            DoctorId = doctorId,
            TemplateName = request.TemplateName,
            Diagnosis = request.Diagnosis,
            ClinicalNotes = request.ClinicalNotes,
            TreatmentPlan = request.TreatmentPlan,
            FollowUpInstructions = request.FollowUpInstructions
        };

        await _repository.AddAsync(template, cancellationToken);

        return template.Id;
    }
}
