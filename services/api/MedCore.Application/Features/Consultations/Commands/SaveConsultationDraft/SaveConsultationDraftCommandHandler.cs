using System;
using System.Threading;
using System.Threading.Tasks;
using MedCore.Application.Common;
using MedCore.Domain.Entities.Clinical;
using MediatR;

namespace MedCore.Application.Features.Consultations.Commands.SaveConsultationDraft;

public class SaveConsultationDraftCommandHandler : IRequestHandler<SaveConsultationDraftCommand, Unit>
{
    private readonly IGenericRepository<Consultation> _consultationRepository;

    public SaveConsultationDraftCommandHandler(IGenericRepository<Consultation> consultationRepository)
    {
        _consultationRepository = consultationRepository;
    }

    public async Task<Unit> Handle(SaveConsultationDraftCommand request, CancellationToken cancellationToken)
    {
        var consultation = await _consultationRepository.GetByIdAsync(request.ConsultationId, cancellationToken);
        
        if (consultation == null || consultation.DoctorId != request.DoctorId)
            throw new Exception("Consultation not found or unauthorized.");

        if (consultation.IsFinalized)
            throw new Exception("Cannot edit a finalized consultation.");

        consultation.ChiefComplaint = request.ChiefComplaint ?? consultation.ChiefComplaint;
        consultation.Symptoms = request.Symptoms ?? consultation.Symptoms;
        consultation.Diagnosis = request.Diagnosis ?? consultation.Diagnosis;
        consultation.TreatmentPlan = request.TreatmentPlan ?? consultation.TreatmentPlan;
        
        consultation.ClinicalNotes = request.ClinicalNotes;
        consultation.PrivateNotes = request.PrivateNotes;
        consultation.PatientNotes = request.PatientNotes;
        
        consultation.VisitType = request.VisitType;
        consultation.FollowUpDate = request.FollowUpDate;
        consultation.FollowUpUrgency = request.FollowUpUrgency;
        consultation.FollowUpInstructions = request.FollowUpInstructions;

        await _consultationRepository.UpdateAsync(consultation, cancellationToken);

        return Unit.Value;
    }
}
