using MedicHp.Application.Features.WhatsApp;

namespace MedicHp.Application.Common;

/// <summary>
/// Bounded channel-based queue for WhatsApp webhook events.
/// The webhook controller enqueues payloads and returns HTTP 200 immediately.
/// A BackgroundService dequeues and processes them asynchronously.
/// </summary>
public interface IWhatsAppEventQueue
{
    /// <summary>
    /// Enqueue a webhook payload for background processing.
    /// </summary>
    ValueTask EnqueueAsync(WhatsAppWebhookPayload payload, CancellationToken ct = default);

    /// <summary>
    /// Dequeue the next webhook payload. Blocks until one is available.
    /// Called by the background worker.
    /// </summary>
    ValueTask<WhatsAppWebhookPayload> DequeueAsync(CancellationToken ct = default);
}
