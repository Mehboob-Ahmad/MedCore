using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MedCore.Application.Common;
using MedCore.Application.Features.Consultations.Commands.StartConsultation;
using MedCore.Domain.Entities.Clinical;
using Moq;
using Xunit;
using MockQueryable.Moq;
using System.Collections.Generic;

namespace MedCore.Application.UnitTests.Features.Consultations;

public class StartConsultationCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Appointment>> _appointmentRepoMock;
    private readonly Mock<IGenericRepository<Consultation>> _consultationRepoMock;
    private readonly StartConsultationCommandHandler _handler;

    public StartConsultationCommandHandlerTests()
    {
        _appointmentRepoMock = new Mock<IGenericRepository<Appointment>>();
        _consultationRepoMock = new Mock<IGenericRepository<Consultation>>();
        _handler = new StartConsultationCommandHandler(_appointmentRepoMock.Object, _consultationRepoMock.Object);
    }

    [Fact]
    public async Task Handle_WhenAppointmentNotFound_ShouldThrowException()
    {
        var command = new StartConsultationCommand { AppointmentId = Guid.NewGuid(), DoctorId = Guid.NewGuid() };
        var appointments = new List<Appointment>().AsQueryable().BuildMock();
        _appointmentRepoMock.Setup(r => r.GetQueryable()).Returns(appointments);

        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<Exception>().WithMessage("Appointment not found or unauthorized.");
    }
}
