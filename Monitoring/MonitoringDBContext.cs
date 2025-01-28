using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Monitoring.Models;
using Monitoring.Models.NotificationsModule;

public class MonitoringDbContext : IdentityDbContext<IdentityUser>
{
     public DbSet<Client> Clients { get; set; }
    public DbSet<Website> Websites { get; set; }
    public DbSet<NotificationLog> NotificationLogs { get; set; }
    public DbSet<NotificationPreferences> NotificationPreferences { get; set; }
    public DbSet<CheckResults> CheckResults { get; set; }
    public DbSet<Analytics> Analytics { get; set; }

    public MonitoringDbContext(DbContextOptions<MonitoringDbContext> options)
        : base(options) { }

}
