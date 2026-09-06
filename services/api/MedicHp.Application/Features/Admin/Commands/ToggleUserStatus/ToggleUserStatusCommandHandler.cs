using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Core;
using MedicHp.Shared.Exceptions;
using MediatR;

namespace MedicHp.Application.Features.Admin.Commands.ToggleUserStatus;

public class ToggleUserStatusCommandHandler : IRequestHandler<ToggleUserStatusCommand, bool>
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ToggleUserStatusCommandHandler(IGenericRepository<User> userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), request.UserId);

        user.AccountStatus = request.AccountStatus;
        user.SuspensionReason = request.AccountStatus == MedicHp.Domain.Enums.AccountStatus.Frozen ? request.Reason : null;
        user.UpdatedAt = System.DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
