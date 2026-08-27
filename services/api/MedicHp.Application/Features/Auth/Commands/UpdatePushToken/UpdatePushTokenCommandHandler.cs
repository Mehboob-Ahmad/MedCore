using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Core;
using MedicHp.Application.Features.Auth.Interfaces;

namespace MedicHp.Application.Features.Auth.Commands.UpdatePushToken;

public class UpdatePushTokenCommandHandler : IRequestHandler<UpdatePushTokenCommand, bool>
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdatePushTokenCommandHandler(
        IGenericRepository<User> userRepository, 
        IUnitOfWork unitOfWork, 
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(UpdatePushTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            throw new UnauthorizedAccessException("User not authenticated.");
        }

        var user = await _userRepository.GetByIdAsync(userId.Value);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        user.PushToken = request.PushToken;
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
