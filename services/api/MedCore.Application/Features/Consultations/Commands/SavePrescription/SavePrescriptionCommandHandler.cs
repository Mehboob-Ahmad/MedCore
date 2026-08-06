using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedCore.Application.Common;
using MedCore.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Application.Features.Consultations.Commands.SavePrescription;

public class SavePrescriptionCommandHandler : IRequestHandler<SavePrescriptionCommand, Guid>
{
    private readonly IGenericRepository<Consultation> _consultationRepository;
    private readonly IGenericRepository<Prescription> _prescriptionRepository;

    public SavePrescriptionCommandHandler(
        IGenericRepository<Consultation> consultationRepository,
        IGenericRepository<Prescription> prescriptionRepository)
    {
        _consultationRepository = consultationRepository;
        _prescriptionRepository = prescriptionRepository;
    }

    public async Task<Guid> Handle(SavePrescriptionCommand request, CancellationToken cancellationToken)
    {
        var consultation = await _consultationRepository.GetQueryable()
            .Include(c => c.Prescriptions)
                .ThenInclude(p => p.Items)
            .FirstOrDefaultAsync(c => c.Id == request.ConsultationId, cancellationToken);

        if (consultation == null || consultation.DoctorId != request.DoctorId)
            throw new Exception("Consultation not found or unauthorized.");

        if (consultation.IsFinalized)
            throw new Exception("Cannot edit a prescription for a finalized consultation.");

        var existingPrescription = consultation.Prescriptions.FirstOrDefault(p => !p.IsSuperseded);
        
        if (existingPrescription == null)
        {
            existingPrescription = new Prescription
            {
                ConsultationId = consultation.Id,
                DoctorId = consultation.DoctorId,
                PatientId = consultation.PatientId,
                IssuedAt = DateTime.UtcNow,
                Notes = request.Notes
            };
            await _prescriptionRepository.AddAsync(existingPrescription, cancellationToken);
        }
        else
        {
            existingPrescription.Notes = request.Notes;
            existingPrescription.IssuedAt = DateTime.UtcNow;
            existingPrescription.Items.Clear(); // Replace items completely
        }

        foreach (var itemDto in request.Items.OrderBy(i => i.SortOrder))
        {
            existingPrescription.Items.Add(new PrescriptionItem
            {
                MedicationName = itemDto.MedicationName,
                Strength = itemDto.Strength,
                Dosage = itemDto.Dosage,
                Frequency = itemDto.Frequency,
                Duration = itemDto.Duration,
                Route = itemDto.Route,
                Timing = itemDto.Timing,
                Quantity = itemDto.Quantity,
                Instructions = itemDto.Instructions,
                SortOrder = itemDto.SortOrder
            });
        }

        await _consultationRepository.UpdateAsync(consultation, cancellationToken);

        return existingPrescription.Id;
    }
}
