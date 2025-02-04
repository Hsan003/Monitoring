namespace Monitoring.Models.NotificationsModule;

public class NotificationManager
{
    private static NotificationManager _instance;
    private NotificationManager() {
     }

    public static NotificationManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new NotificationManager();
        }
        return _instance;
    }

    public async void NotifyClient(Client client, string message)
    {
        foreach (var channel in client.NotifChannels)
        {
            switch (channel)
            {
                
                // case "SMS":
                //     await _smsChannel.SendNotification(message, client);
                //     break;
                case "Email":
                default:
                    EmailChannel em = new();
                    await em.SendNotification(message, client);
                    break;
            }
            // channel.SendNotification(message, client);
        }
    }

    public void LogNotification(string message)
    {
        // Implement logging (e.g., using NLog)
        Console.WriteLine("Logging Notification: " + message);
    }
}
