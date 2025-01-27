using System.ComponentModel.DataAnnotations;

namespace Monitoring.Models.NotificationsModule;

public class NotificationLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Timestamp { get; set; }

    [Required]
    [MaxLength(500)]
    public string Content { get; set; }

    [Required]
    [MaxLength(50)]
    public string DeliveryStatus { get; set; } // e.g., Success, Failed

    public NotificationLog(){}

    public NotificationLog(string content, string status)
    {
        Timestamp = DateTime.Now.ToString();
        Content = content;
        DeliveryStatus = status;
    }

    public void LogNotification()
    {
        Console.WriteLine($"{Timestamp} - {Content} - {DeliveryStatus}");
    }
}
