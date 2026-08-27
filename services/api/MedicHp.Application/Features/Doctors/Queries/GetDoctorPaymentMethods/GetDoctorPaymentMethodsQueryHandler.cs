using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Shared.Exceptions;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Doctors.DTOs;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedicHp.Application.Features.Doctors.Queries.GetDoctorPaymentMethods;

public class GetDoctorPaymentMethodsQueryHandler : IRequestHandler<GetDoctorPaymentMethodsQuery, List<DoctorPaymentMethodDto>>
{
    private readonly IGenericRepository<DoctorProfile> _doctorProfileRepository;

    public GetDoctorPaymentMethodsQueryHandler(IGenericRepository<DoctorProfile> doctorProfileRepository)
    {
        _doctorProfileRepository = doctorProfileRepository;
    }

    public async Task<List<DoctorPaymentMethodDto>> Handle(GetDoctorPaymentMethodsQuery request, CancellationToken cancellationToken)
    {
        var profile = await _doctorProfileRepository.FirstOrDefaultAsync(
            p => p.UserId == request.DoctorId,
            include: q => q.Include(p => p.PaymentMethods),
            cancellationToken: cancellationToken);

        if (profile == null)
        {
            throw new NotFoundException(nameof(DoctorProfile), request.DoctorId);
        }

        return profile.PaymentMethods.Where(pm => pm.IsActive).Select(pm => new DoctorPaymentMethodDto
        {
            Id = pm.Id,
            PaymentMethodType = pm.PaymentMethodType,
            PaymentProvider = pm.PaymentProvider,
            AccountTitle = pm.AccountTitle,
            // Mask account number to show only last 4 digits for query, 
            // but since doctor owns this, maybe they need to see full? 
            // In GetDoctorProfile we mask it. Here, it depends on caller. Let's return full for the doctor's own dashboard.
            AccountNumber = pm.AccountNumber, 
            IBAN = pm.IBAN,
            IsActive = pm.IsActive
        }).ToList();
    }
}
