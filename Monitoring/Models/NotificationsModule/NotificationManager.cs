namespace Monitoring.Models.NotificationsModule;

public class NotificationManager
{
    private static NotificationManager _instance;

    private NotificationManager() { }

    public static NotificationManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new NotificationManager();
        }
        return _instance;
    }

    public void NotifyClient(Client client, string message)
    {
        foreach (var channel in client.Preferences.Channels)
        {
            channel.SendNotification(message, client);
        }
    }

    public void LogNotification(string message)
    {
        // Implement logging (e.g., using NLog)
        Console.WriteLine("Logging Notification: " + message);
    }
}
