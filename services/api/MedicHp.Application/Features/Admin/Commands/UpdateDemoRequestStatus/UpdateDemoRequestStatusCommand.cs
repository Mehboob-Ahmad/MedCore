using System;
using MediatR;
using MedicHp.Domain.Entities.Admin;

namespace MedicHp.Application.Features.Admin.Commands.UpdateDemoRequestStatus;

public class UpdateDemoRequestStatusCommand : IRequest<bool>
{
    public Guid RequestId { get; set; }
    public DemoRequestStatus Status { get; set; }
}
