using System;
using MediatR;

namespace MedicHp.Application.Features.Consultations.Commands.SaveConsultationDraft;

public class SaveConsultationDraftCommand : IRequest<Unit>
{
    public Guid ConsultationId { get; set; }
    public Guid DoctorId { get; set; }

    public string? ChiefComplaint { get; set; }
    public string? Symptoms { get; set; }
    public string? Diagnosis { get; set; }
    public string? TreatmentPlan { get; set; }
    
    // Notes
    public string? ClinicalNotes { get; set; }
    public string? PrivateNotes { get; set; }
    public string? PatientNotes { get; set; }
    
    // Follow up
    public string? VisitType { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public string? FollowUpUrgency { get; set; }
    public string? FollowUpInstructions { get; set; }
}
