using System;
using MediatR;

namespace MedicHp.Application.Features.Templates.Commands.CreateConsultationTemplate;

public class CreateConsultationTemplateCommand : IRequest<Guid>
{
    public string TemplateName { get; set; } = null!;
    public string? Diagnosis { get; set; }
    public string? ClinicalNotes { get; set; }
    public string? TreatmentPlan { get; set; }
    public string? FollowUpInstructions { get; set; }
}
