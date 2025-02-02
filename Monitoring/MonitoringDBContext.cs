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
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // ✅ Ensure Identity tables are properly created

        // Define relationship: Analytics -> Website
        modelBuilder.Entity<Analytics>()
            .HasOne(a => a.Website)
            .WithMany()  // Website does NOT have a collection of Analytics
            .HasForeignKey(a => a.WebsiteId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public MonitoringDbContext(DbContextOptions<MonitoringDbContext> options)
        : base(options) { }

}
