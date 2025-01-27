
namespace Monitoring.Models.NotificationsModule.NotificationsChannels;

public interface INotificationChannel
{
    public abstract Task<bool> SendNotification(string content, Client recipient);
}
