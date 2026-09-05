using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Doctors.Commands.ConfigurePaymentMethods;
using MedicHp.Application.Features.Doctors.DTOs;
using MedicHp.Domain.Entities.Clinical;
using MedicHp.Domain.Enums;
using MedicHp.Shared.Exceptions;
using Moq;
using Xunit;

namespace MedicHp.Application.UnitTests.Features.Doctors.Commands;

public class ConfigurePaymentMethodsCommandHandlerTests
{
    private readonly Mock<IGenericRepository<DoctorProfile>> _mockProfileRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly ConfigurePaymentMethodsCommandHandler _handler;

    public ConfigurePaymentMethodsCommandHandlerTests()
    {
        _mockProfileRepository = new Mock<IGenericRepository<DoctorProfile>>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new ConfigurePaymentMethodsCommandHandler(_mockProfileRepository.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ConfiguresPaymentMethods()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var doctorProfile = new DoctorProfile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PaymentMethods = new List<DoctorPaymentMethod>()
        };

        _mockProfileRepository.Setup(x => x.FirstOrDefaultAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<DoctorProfile, bool>>>(),
            It.IsAny<Func<System.Linq.IQueryable<DoctorProfile>, System.Linq.IQueryable<DoctorProfile>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctorProfile);

        var command = new ConfigurePaymentMethodsCommand
        {
            UserId = userId,
            PaymentMethods = new List<PaymentMethodInputDto>
            {
                new PaymentMethodInputDto
                {
                    PaymentMethodType = PaymentMethodType.BankTransfer,
                    PaymentProvider = "Test Bank",
                    AccountTitle = "Dr. Test",
                    AccountNumber = "123456",
                    IsActive = true
                }
            }
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Single(doctorProfile.PaymentMethods);
        Assert.Equal("Test Bank", doctorProfile.PaymentMethods.First().PaymentProvider);
        _mockProfileRepository.Verify(x => x.UpdateAsync(doctorProfile, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ProfileNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = new ConfigurePaymentMethodsCommand
        {
            UserId = Guid.NewGuid(),
            PaymentMethods = new List<PaymentMethodInputDto>()
        };

        _mockProfileRepository.Setup(x => x.FirstOrDefaultAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<DoctorProfile, bool>>>(),
            It.IsAny<Func<System.Linq.IQueryable<DoctorProfile>, System.Linq.IQueryable<DoctorProfile>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((DoctorProfile)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
