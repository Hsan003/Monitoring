
using System.ComponentModel.DataAnnotations;
using Monitoring.Models.NotificationsModule;

namespace Monitoring.Models;

public class Client
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    public string Email { get; set; }

    public virtual ICollection<Website> Websites { get; set; } = new List<Website>();

    [Required]
    public virtual NotificationPreferences Preferences { get; set; }
}
