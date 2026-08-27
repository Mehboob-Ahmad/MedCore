using System;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Clinical;
using MedicHp.Domain.Entities.Core;
using MedicHp.Infrastructure.Services;
using MedicHp.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace MedicHp.Application.UnitTests.Services;

public class WhatsAppNotificationServiceTests
{
    private readonly Mock<IWhatsAppService> _mockWhatsAppService;
    private readonly Mock<IGenericRepository<Appointment>> _mockApptRepo;
    private readonly IOptions<WhatsAppTemplateSettings> _mockSettings;
    private readonly Mock<ILogger<WhatsAppNotificationService>> _mockLogger;
    private readonly WhatsAppNotificationService _sut;

    public WhatsAppNotificationServiceTests()
    {
        _mockWhatsAppService = new Mock<IWhatsAppService>();
        _mockApptRepo = new Mock<IGenericRepository<Appointment>>();
        _mockSettings = Options.Create(new WhatsAppTemplateSettings());
        _mockLogger = new Mock<ILogger<WhatsAppNotificationService>>();

        _sut = new WhatsAppNotificationService(
            _mockWhatsAppService.Object,
            _mockApptRepo.Object,
            _mockSettings,
            _mockLogger.Object);
    }

    [Fact]
    public async Task SendPaymentReminderAsync_FormatsPayloadCorrectly()
    {
        // Arrange
        var apptId = Guid.NewGuid();
        var amount = 2500m;
        var appt = new Appointment
        {
            Id = apptId,
            ScheduledAt = new DateTime(2026, 8, 25, 17, 0, 0),
            Patient = new User { FirstName = "Mehboob", LastName = "Ahmad", PhoneNumber = "03271234567" },
            Doctor = new User { LastName = "Ahmed" }
        };

        _mockApptRepo.Setup(x => x.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Appointment, bool>>>(),
                It.IsAny<Func<System.Linq.IQueryable<Appointment>, System.Linq.IQueryable<Appointment>>>(),
                CancellationToken.None))
            .ReturnsAsync(appt);

        // Act
        await _sut.SendPaymentReminderAsync(apptId, amount);

        // Assert
        _mockWhatsAppService.Verify(x => x.SendTemplateMessageAsync(
            "03271234567",
            "payment_reminder",
            "en",
            It.Is<object[]>(c => c.Length == 1),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }
}
