using System;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Appointments.Commands.ConfirmPayment;
using MedicHp.Domain.Entities.Clinical;
using MedicHp.Domain.Enums;
using MedicHp.Shared.Exceptions;
using Moq;
using Xunit;

namespace MedicHp.Application.UnitTests.Features.Appointments.Commands;

public class ConfirmPaymentCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Appointment>> _mockAppointmentRepository;
    private readonly Mock<IGenericRepository<MedicHp.Domain.Entities.Core.Notification>> _mockNotificationRepository;
    private readonly Mock<IWhatsAppNotificationService> _mockWhatsAppService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly ConfirmPaymentCommandHandler _handler;

    public ConfirmPaymentCommandHandlerTests()
    {
        _mockAppointmentRepository = new Mock<IGenericRepository<Appointment>>();
        _mockNotificationRepository = new Mock<IGenericRepository<MedicHp.Domain.Entities.Core.Notification>>();
        _mockWhatsAppService = new Mock<IWhatsAppNotificationService>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _handler = new ConfirmPaymentCommandHandler(
            _mockAppointmentRepository.Object,
            _mockNotificationRepository.Object,
            _mockWhatsAppService.Object,
            _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ConfirmsPaymentAndSendsWhatsApp()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var appointmentId = Guid.NewGuid();
        
        var appointment = new Appointment
        {
            Id = appointmentId,
            DoctorId = doctorId,
            PaymentStatus = PaymentStatus.Pending.ToString()
        };

        _mockAppointmentRepository.Setup(x => x.GetByIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        var command = new ConfirmPaymentCommand
        {
            DoctorId = doctorId,
            AppointmentId = appointmentId
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(PaymentStatus.Paid.ToString(), appointment.PaymentStatus);
        Assert.NotNull(appointment.PaymentConfirmedAt);
        Assert.Equal(doctorId, appointment.PaymentConfirmedByUserId);

        _mockAppointmentRepository.Verify(x => x.UpdateAsync(appointment, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockWhatsAppService.Verify(x => x.SendPaymentSuccessAsync(appointmentId, 0m, It.IsAny<CancellationToken>()), Times.Once);
        _mockNotificationRepository.Verify(x => x.AddAsync(It.IsAny<MedicHp.Domain.Entities.Core.Notification>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AppointmentNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = new ConfirmPaymentCommand
        {
            DoctorId = Guid.NewGuid(),
            AppointmentId = Guid.NewGuid()
        };

        _mockAppointmentRepository.Setup(x => x.GetByIdAsync(command.AppointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_UnauthorizedDoctor_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var appointment = new Appointment
        {
            Id = appointmentId,
            DoctorId = Guid.NewGuid(), // Different doctor
            PaymentStatus = PaymentStatus.Pending.ToString()
        };

        _mockAppointmentRepository.Setup(x => x.GetByIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        var command = new ConfirmPaymentCommand
        {
            DoctorId = Guid.NewGuid(),
            AppointmentId = appointmentId
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, CancellationToken.None));
    }
    
    [Fact]
    public async Task Handle_AlreadyPaid_ThrowsInvalidOperationException()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var appointmentId = Guid.NewGuid();
        
        var appointment = new Appointment
        {
            Id = appointmentId,
            DoctorId = doctorId,
            PaymentStatus = PaymentStatus.Paid.ToString() // Already paid
        };

        _mockAppointmentRepository.Setup(x => x.GetByIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        var command = new ConfirmPaymentCommand
        {
            DoctorId = doctorId,
            AppointmentId = appointmentId
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("Payment is already confirmed for this appointment.", ex.Message);
    }
}
