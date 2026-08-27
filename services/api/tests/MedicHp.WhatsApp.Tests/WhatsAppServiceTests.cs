using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Clinical;
using MedicHp.Domain.Entities.Core;
using MedicHp.Domain.Entities.Messaging;
using MedicHp.Infrastructure.Services;
using MedicHp.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace MedicHp.WhatsApp.Tests;

public class WhatsAppServiceTests
{
    private readonly Mock<IGenericRepository<DoctorProfile>> _mockDoctorRepo;
    private readonly Mock<IGenericRepository<WhatsAppMessage>> _mockMessageRepo;
    private readonly Mock<IGenericRepository<User>> _mockUserRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly IOptions<WhatsAppSettings> _mockSettings;
    private readonly Mock<ILogger<WhatsAppService>> _mockLogger;

    public WhatsAppServiceTests()
    {
        _mockDoctorRepo = new Mock<IGenericRepository<DoctorProfile>>();
        _mockMessageRepo = new Mock<IGenericRepository<WhatsAppMessage>>();
        _mockUserRepo = new Mock<IGenericRepository<User>>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockSettings = Options.Create(new WhatsAppSettings 
        { 
            ApiVersion = "v21.0",
            PhoneNumberId = "1269638122899661",
            GlobalAccessToken = "dummy_token"
        });
        _mockLogger = new Mock<ILogger<WhatsAppService>>();
    }

    [Theory]
    [InlineData("03271234567", "923271234567")]
    [InlineData("+923271234567", "923271234567")]
    [InlineData("923271234567", "923271234567")]
    public async Task SendTextMessageAsync_NormalizesPhoneNumber(string inputPhone, string expectedPhone)
    {
        string requestPayload = string.Empty;

        // We will use a dummy HttpMessageHandler to intercept the request and verify the payload
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                if (request.Content != null)
                {
                    requestPayload = await request.Content.ReadAsStringAsync(token);
                }
                
                return new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("{\"messages\":[{\"id\":\"wamid_123\"}]}")
                };
            });

        var client = new HttpClient(handlerMock.Object);

        var service = new WhatsAppService(
            client,
            _mockDoctorRepo.Object,
            _mockMessageRepo.Object,
            _mockUserRepo.Object,
            _mockUnitOfWork.Object,
            _mockSettings,
            _mockLogger.Object);

        // Act
        var result = await service.SendTextMessageAsync(inputPhone, "Hello", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        // Verify that the Http request payload actually had the expected phone number
        Assert.Contains($"\"to\":\"{expectedPhone}\"", requestPayload);
    }
}
