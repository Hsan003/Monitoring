using Microsoft.EntityFrameworkCore;
using Monitoring.Models;
using Monitoring.Models.NotificationsModule;

public class MonitoringDbContext : DbContext
{
     public DbSet<Client> Clients { get; set; }
    public DbSet<Website> Websites { get; set; }
    public DbSet<NotificationLog> NotificationLogs { get; set; }
    public DbSet<NotificationPreferences> NotificationPreferences { get; set; }

    public MonitoringDbContext(DbContextOptions<MonitoringDbContext> options)
        : base(options) { }

}
