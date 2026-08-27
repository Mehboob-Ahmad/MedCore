using System.Threading;
using System.Threading.Tasks;
using MedicHp.Shared.Exceptions;
using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedicHp.Application.Features.Doctors.Commands.ConfigurePaymentMethods;

public class ConfigurePaymentMethodsCommandHandler : IRequestHandler<ConfigurePaymentMethodsCommand, bool>
{
    private readonly IGenericRepository<DoctorProfile> _doctorProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfigurePaymentMethodsCommandHandler(
        IGenericRepository<DoctorProfile> doctorProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _doctorProfileRepository = doctorProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ConfigurePaymentMethodsCommand request, CancellationToken cancellationToken)
    {
        var profile = await _doctorProfileRepository.FirstOrDefaultAsync(
            d => d.UserId == request.UserId,
            include: q => q.Include(p => p.PaymentMethods),
            cancellationToken: cancellationToken);

        if (profile == null)
        {
            throw new NotFoundException(nameof(DoctorProfile), request.UserId);
        }

        // Clear existing and replace with new ones
        profile.PaymentMethods.Clear();
        
        foreach (var pm in request.PaymentMethods)
        {
            profile.PaymentMethods.Add(new DoctorPaymentMethod
            {
                PaymentMethodType = pm.PaymentMethodType,
                PaymentProvider = pm.PaymentProvider,
                AccountTitle = pm.AccountTitle,
                AccountNumber = pm.AccountNumber,
                IBAN = pm.IBAN,
                IsActive = pm.IsActive
            });
        }

        await _doctorProfileRepository.UpdateAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
