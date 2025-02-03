
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Monitoring.Models.NotificationsModule;

namespace Monitoring.Models;

[Table("AspNetUsers")]

public class Client : IdentityUser
{

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    public virtual ICollection<Website> Websites { get; set; } = new List<Website>();

    [Required]
    public virtual NotificationPreferences Preferences { get; set; }
}
