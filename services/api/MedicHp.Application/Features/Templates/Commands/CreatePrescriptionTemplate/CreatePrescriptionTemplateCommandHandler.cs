using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Auth.Interfaces;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedicHp.Application.Features.Templates.Commands.CreatePrescriptionTemplate;

public class CreatePrescriptionTemplateCommandHandler : IRequestHandler<CreatePrescriptionTemplateCommand, Guid>
{
    private readonly IGenericRepository<PrescriptionTemplate> _repository;
    private readonly ICurrentUserService _currentUserService;

    public CreatePrescriptionTemplateCommandHandler(
        IGenericRepository<PrescriptionTemplate> repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreatePrescriptionTemplateCommand request, CancellationToken cancellationToken)
    {
        var doctorId = _currentUserService.UserId!.Value;

        var exists = await _repository.GetQueryable()
            .AnyAsync(t => t.DoctorId == doctorId && t.TemplateName == request.TemplateName, cancellationToken);
            
        if (exists)
        {
            throw new Exception($"Prescription Template with name '{request.TemplateName}' already exists.");
        }

        var template = new PrescriptionTemplate
        {
            DoctorId = doctorId,
            TemplateName = request.TemplateName,
            Notes = request.Notes,
            Items = request.Items.Select(i => new PrescriptionTemplateItem
            {
                MedicationName = i.MedicationName,
                Strength = i.Strength,
                Dosage = i.Dosage,
                Frequency = i.Frequency,
                Duration = i.Duration,
                Route = i.Route,
                Timing = i.Timing,
                Quantity = i.Quantity,
                Instructions = i.Instructions
            }).ToList()
        };

        await _repository.AddAsync(template, cancellationToken);

        return template.Id;
    }
}
