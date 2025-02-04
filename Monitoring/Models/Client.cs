
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Monitoring.Models.NotificationsModule;
using Monitoring.Models.NotificationsModule.NotificationsChannels;

namespace Monitoring.Models;

[Table("AspNetUsers")]

public class Client : IdentityUser
{

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    public virtual ICollection<Website> Websites { get; set; } = new List<Website>();

    public virtual List<string> NotifChannels { get; set; }

    public Client()
    {
        NotifChannels = new List<string>();
    }
}
