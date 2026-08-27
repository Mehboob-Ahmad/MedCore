using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using Microsoft.Extensions.Logging;

namespace MedicHp.Infrastructure.Services;

public class ExpoPushNotificationService : IPushNotificationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExpoPushNotificationService> _logger;

    public ExpoPushNotificationService(HttpClient httpClient, ILogger<ExpoPushNotificationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task SendPushNotificationAsync(string pushToken, string title, string body, object? data = null)
    {
        if (string.IsNullOrWhiteSpace(pushToken) || !pushToken.StartsWith("ExponentPushToken["))
        {
            _logger.LogWarning("Invalid Expo push token: {PushToken}", pushToken);
            return;
        }

        var message = new
        {
            to = pushToken,
            sound = "default",
            title = title,
            body = body,
            data = data
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("https://exp.host/--/api/v2/push/send", message);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to send push notification to {PushToken}. Status: {StatusCode}. Response: {Response}", pushToken, response.StatusCode, errorContent);
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error occurred while sending push notification to {PushToken}", pushToken);
        }
    }
}
