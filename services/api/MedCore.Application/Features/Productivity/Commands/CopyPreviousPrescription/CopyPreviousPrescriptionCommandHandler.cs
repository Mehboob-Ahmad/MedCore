using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedCore.Application.Common;
using MedCore.Application.Features.Auth.Interfaces;
using MedCore.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Application.Features.Productivity.Commands.CopyPreviousPrescription;

public class CopyPreviousPrescriptionCommandHandler : IRequestHandler<CopyPreviousPrescriptionCommand, bool>
{
    private readonly IGenericRepository<Prescription> _prescriptionRepo;
    private readonly IGenericRepository<Consultation> _consultationRepo;
    private readonly ICurrentUserService _currentUserService;

    public CopyPreviousPrescriptionCommandHandler(
        IGenericRepository<Prescription> prescriptionRepo,
        IGenericRepository<Consultation> consultationRepo,
        ICurrentUserService currentUserService)
    {
        _prescriptionRepo = prescriptionRepo;
        _consultationRepo = consultationRepo;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(CopyPreviousPrescriptionCommand request, CancellationToken cancellationToken)
    {
        var doctorId = _currentUserService.UserId!.Value;

        // Verify target consultation exists and belongs to doctor, and is not finalized
        var targetConsultation = await _consultationRepo.GetQueryable()
            .FirstOrDefaultAsync(c => c.Id == request.TargetConsultationId && c.DoctorId == doctorId, cancellationToken);
            
        if (targetConsultation == null || targetConsultation.IsFinalized)
        {
            throw new Exception("Target consultation not found or is already finalized.");
        }

        // Get source prescription
        var sourcePrescription = await _prescriptionRepo.GetQueryable()
            .Include(p => p.Items)
            .Include(p => p.Consultation)
            .FirstOrDefaultAsync(p => p.ConsultationId == request.SourceConsultationId, cancellationToken);

        if (sourcePrescription == null)
        {
            throw new Exception("Source prescription not found.");
        }

        // Ensure both consultations belong to the same patient
        if (sourcePrescription.Consultation.PatientId != targetConsultation.PatientId)
        {
            throw new Exception("Cannot copy a prescription from a different patient.");
        }

        // Check if target already has a prescription
        var targetPrescription = await _prescriptionRepo.GetQueryable()
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.ConsultationId == request.TargetConsultationId, cancellationToken);

        if (targetPrescription != null)
        {
            // Update existing prescription
            targetPrescription.Notes = sourcePrescription.Notes;
            targetPrescription.Items.Clear();
            
            foreach (var item in sourcePrescription.Items)
            {
                targetPrescription.Items.Add(new PrescriptionItem
                {
                    MedicationName = item.MedicationName,
                    Strength = item.Strength,
                    Dosage = item.Dosage,
                    Frequency = item.Frequency,
                    Duration = item.Duration,
                    Route = item.Route,
                    Timing = item.Timing,
                    Quantity = item.Quantity,
                    Instructions = item.Instructions
                });
            }
            
            await _prescriptionRepo.UpdateAsync(targetPrescription, cancellationToken);
        }
        else
        {
            // Create new prescription
            var newPrescription = new Prescription
            {
                ConsultationId = request.TargetConsultationId,
                Notes = sourcePrescription.Notes,
                Items = sourcePrescription.Items.Select(item => new PrescriptionItem
                {
                    MedicationName = item.MedicationName,
                    Strength = item.Strength,
                    Dosage = item.Dosage,
                    Frequency = item.Frequency,
                    Duration = item.Duration,
                    Route = item.Route,
                    Timing = item.Timing,
                    Quantity = item.Quantity,
                    Instructions = item.Instructions
                }).ToList()
            };
            
            await _prescriptionRepo.AddAsync(newPrescription, cancellationToken);
        }

        return true;
    }
}
