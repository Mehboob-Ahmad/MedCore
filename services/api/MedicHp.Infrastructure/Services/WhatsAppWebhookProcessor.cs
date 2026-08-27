using MedicHp.Application.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MedicHp.Infrastructure.Services;

/// <summary>
/// Background worker that processes WhatsApp webhooks asynchronously.
/// Dequeues payloads from the IWhatsAppEventQueue and routes them to IWhatsAppService.
/// 
/// This ensures the HTTP POST endpoint returns 200 OK to Meta immediately,
/// while processing (DB lookups, saves, routing) happens without blocking.
/// </summary>
public class WhatsAppWebhookProcessor : BackgroundService
{
    private readonly IWhatsAppEventQueue _queue;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<WhatsAppWebhookProcessor> _logger;

    public WhatsAppWebhookProcessor(
        IWhatsAppEventQueue queue,
        IServiceProvider serviceProvider,
        ILogger<WhatsAppWebhookProcessor> logger)
    {
        _queue = queue;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("WhatsAppWebhookProcessor background service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Wait for a payload to be enqueued
                var payload = await _queue.DequeueAsync(stoppingToken);

                // Create a new DI scope for processing (to resolve scoped DbContext etc)
                using var scope = _serviceProvider.CreateScope();
                var whatsappService = scope.ServiceProvider.GetRequiredService<IWhatsAppService>();

                // Process the webhook
                await whatsappService.ProcessWebhookAsync(payload, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Graceful shutdown
                break;
            }
            catch (Exception ex)
            {
                // Catch all other exceptions so the background worker doesn't crash
                _logger.LogError(ex, "Error occurred processing WhatsApp webhook in background service.");
            }
        }

        _logger.LogInformation("WhatsAppWebhookProcessor background service is stopping.");
    }
}
