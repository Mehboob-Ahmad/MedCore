using System.Security.Claims;
using System.Text.Json;
using MedicHp.API.Controllers.v1;
using MedicHp.Application.Common;
using MedicHp.Application.Features.WhatsApp;
using MedicHp.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace MedicHp.WhatsApp.Tests;

public class WhatsAppWebhookTests
{
    private readonly Mock<IWhatsAppEventQueue> _mockQueue;
    private readonly IOptions<WhatsAppSettings> _mockSettings;
    private readonly Mock<ILogger<WhatsAppWebhookController>> _mockLogger;
    private readonly WhatsAppWebhookController _controller;

    public WhatsAppWebhookTests()
    {
        _mockQueue = new Mock<IWhatsAppEventQueue>();
        _mockSettings = Options.Create(new WhatsAppSettings
        {
            VerifyToken = "valid_token",
            ApiVersion = "v21.0"
        });
        _mockLogger = new Mock<ILogger<WhatsAppWebhookController>>();
        
        _controller = new WhatsAppWebhookController(
            _mockQueue.Object,
            _mockSettings,
            _mockLogger.Object);
    }

    [Fact]
    public void VerifyWebhook_WithValidToken_ReturnsChallenge()
    {
        // Act
        var result = _controller.VerifyWebhook("subscribe", "valid_token", "challenge123") as ContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("challenge123", result.Content);
        Assert.Equal("text/plain", result.ContentType);
    }

    [Fact]
    public void VerifyWebhook_WithInvalidToken_ReturnsForbidden()
    {
        // Act
        var result = _controller.VerifyWebhook("subscribe", "wrong_token", "challenge123");

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task ReceiveWebhook_WithValidPayload_EnqueuesAndReturnsOk()
    {
        // Arrange
        var payloadJson = """
        {
          "object": "whatsapp_business_account",
          "entry": [
            {
              "id": "123",
              "changes": [
                {
                  "value": {
                    "messaging_product": "whatsapp",
                    "metadata": {
                      "display_phone_number": "12345",
                      "phone_number_id": "12345"
                    }
                  },
                  "field": "messages"
                }
              ]
            }
          ]
        }
        """;
        
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(payloadJson));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { Request = { Body = stream } }
        };

        // Act
        var result = await _controller.ReceiveWebhook();

        // Assert
        Assert.IsType<OkObjectResult>(result);
        _mockQueue.Verify(q => q.EnqueueAsync(It.IsAny<WhatsAppWebhookPayload>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
