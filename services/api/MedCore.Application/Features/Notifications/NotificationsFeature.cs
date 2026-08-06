using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MedCore.Application.Common;
using MedCore.Domain.Entities.Core;
using MediatR;
using MedCore.Application.Features.Auth.Interfaces;
using System.Linq;

namespace MedCore.Application.Features.Notifications.DTOs
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

namespace MedCore.Application.Features.Notifications.Queries.GetNotifications
{
    using MedCore.Application.Features.Notifications.DTOs;
    
    public class GetNotificationsQuery : IRequest<List<NotificationDto>>
    {
        public Guid UserId { get; set; }
        public GetNotificationsQuery(Guid userId) => UserId = userId;
    }

    public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, List<NotificationDto>>
    {
        private readonly IGenericRepository<Notification> _notificationRepository;

        public GetNotificationsQueryHandler(IGenericRepository<Notification> notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<List<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            var notifications = await _notificationRepository.GetAsync(
                n => n.UserId == request.UserId,
                null,
                cancellationToken);

            return notifications.OrderByDescending(n => n.CreatedAt).Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Body,
                Type = n.Type,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList();
        }
    }
}

namespace MedCore.Application.Features.Notifications.Commands.MarkNotificationAsRead
{
    public class MarkNotificationAsReadCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }

    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, bool>
    {
        private readonly IGenericRepository<Notification> _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MarkNotificationAsReadCommandHandler(
            IGenericRepository<Notification> notificationRepository,
            IUnitOfWork unitOfWork)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var notification = await _notificationRepository.GetByIdAsync(request.Id, cancellationToken);
            if (notification == null || notification.UserId != request.UserId)
                throw new MedCore.Shared.Exceptions.NotFoundException(nameof(Notification), request.Id);

            notification.IsRead = true;
            await _notificationRepository.UpdateAsync(notification, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
