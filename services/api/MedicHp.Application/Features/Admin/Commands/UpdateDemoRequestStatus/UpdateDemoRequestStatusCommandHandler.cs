using System;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Admin;
using MedicHp.Shared.Exceptions;
using MediatR;

namespace MedicHp.Application.Features.Admin.Commands.UpdateDemoRequestStatus;

public class UpdateDemoRequestStatusCommandHandler : IRequestHandler<UpdateDemoRequestStatusCommand, bool>
{
    private readonly IGenericRepository<DemoRequest> _demoRequestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDemoRequestStatusCommandHandler(IGenericRepository<DemoRequest> demoRequestRepository, IUnitOfWork unitOfWork)
    {
        _demoRequestRepository = demoRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateDemoRequestStatusCommand request, CancellationToken cancellationToken)
    {
        var demoRequest = await _demoRequestRepository.GetByIdAsync(request.RequestId, cancellationToken);
        if (demoRequest == null)
            throw new NotFoundException(nameof(DemoRequest), request.RequestId);

        demoRequest.Status = request.Status;
        demoRequest.UpdatedAt = DateTime.UtcNow;

        await _demoRequestRepository.UpdateAsync(demoRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
