using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Monitoring.Models.NotificationsModule.NotificationsChannels;

namespace Monitoring.Models.NotificationsModule;

public class NotificationPreferences
{
    public int Id { get; set; }

    [ForeignKey("Client")]
    public int ClientId { get; set; }

    public int LatencyThreshold { get; set; } = 300; // Default latency threshold in ms    

    public int DowntimeThreshold { get; set; } = 30; // Default downtime threshold in seconds

    [Required]
    public string ChannelsJson { get; set; }

    [NotMapped]
    public List<INotificationChannel> Channels
    {
        get => JsonSerializer.Deserialize<List<INotificationChannel>>(ChannelsJson);
        set => ChannelsJson = JsonSerializer.Serialize(value);
    }
    public NotificationPreferences()
    {
        Channels = new List<INotificationChannel>();
    }

    public void AddChannel(INotificationChannel channel)
    {
        Channels.Add(channel);
    }
}
