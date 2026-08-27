using System.Threading.Channels;
using MedicHp.Application.Common;
using MedicHp.Application.Features.WhatsApp;

namespace MedicHp.Infrastructure.Services;

/// <summary>
/// Bounded channel-based queue for WhatsApp webhook events.
///
/// Flow:
///   WhatsAppWebhookController (POST) → EnqueueAsync() → return HTTP 200
///   WhatsAppWebhookProcessor (BackgroundService) → DequeueAsync() → IWhatsAppService.ProcessWebhookAsync()
///
/// Bounded to 1000 items to prevent unbounded memory growth under load.
/// If the queue is full, EnqueueAsync will wait (backpressure).
/// </summary>
public class WhatsAppEventQueue : IWhatsAppEventQueue
{
    private readonly Channel<WhatsAppWebhookPayload> _channel;

    public WhatsAppEventQueue()
    {
        var options = new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        };
        _channel = Channel.CreateBounded<WhatsAppWebhookPayload>(options);
    }

    /// <summary>
    /// Returns the approximate number of items waiting in the queue.
    /// Used for health check reporting.
    /// </summary>
    public int Count => _channel.Reader.Count;

    public async ValueTask EnqueueAsync(WhatsAppWebhookPayload payload, CancellationToken ct = default)
    {
        await _channel.Writer.WriteAsync(payload, ct);
    }

    public async ValueTask<WhatsAppWebhookPayload> DequeueAsync(CancellationToken ct = default)
    {
        return await _channel.Reader.ReadAsync(ct);
    }
}
