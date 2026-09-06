using System;
using MediatR;

namespace MedicHp.Application.Features.Admin.Commands.CreateDoctorFromDemo;

public class CreateDoctorFromDemoCommand : IRequest<bool>
{
    public Guid RequestId { get; set; }
}
