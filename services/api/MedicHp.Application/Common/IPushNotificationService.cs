using System.Threading.Tasks;

namespace MedicHp.Application.Common;

public interface IPushNotificationService
{
    Task SendPushNotificationAsync(string pushToken, string title, string body, object? data = null);
}
