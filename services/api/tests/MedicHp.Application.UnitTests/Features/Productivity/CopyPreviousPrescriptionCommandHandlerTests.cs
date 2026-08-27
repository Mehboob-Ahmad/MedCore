using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Auth.Interfaces;
using MedicHp.Application.Features.Productivity.Commands.CopyPreviousPrescription;
using MedicHp.Domain.Entities.Clinical;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace MedicHp.Application.UnitTests.Features.Productivity;

public class CopyPreviousPrescriptionCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Prescription>> _prescriptionRepoMock;
    private readonly Mock<IGenericRepository<Consultation>> _consultationRepoMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly CopyPreviousPrescriptionCommandHandler _handler;

    public CopyPreviousPrescriptionCommandHandlerTests()
    {
        _prescriptionRepoMock = new Mock<IGenericRepository<Prescription>>();
        _consultationRepoMock = new Mock<IGenericRepository<Consultation>>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        
        _handler = new CopyPreviousPrescriptionCommandHandler(
            _prescriptionRepoMock.Object,
            _consultationRepoMock.Object,
            _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WhenTargetConsultationNotFound_ShouldThrowException()
    {
        var doctorId = Guid.NewGuid();
        _currentUserServiceMock.Setup(s => s.UserId).Returns(doctorId);

        var command = new CopyPreviousPrescriptionCommand { SourceConsultationId = Guid.NewGuid(), TargetConsultationId = Guid.NewGuid() };
        
        var consultations = new List<Consultation>().AsQueryable().BuildMock();
        _consultationRepoMock.Setup(r => r.GetQueryable()).Returns(consultations);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("Target consultation not found or is already finalized.");
    }
}
