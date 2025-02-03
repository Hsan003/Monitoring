using System.ComponentModel.DataAnnotations.Schema;

namespace Monitoring.Models;

public class Analytics
{
    public int Id { get; set; }
    public ICollection<CheckResults> CheckResults { get; set; }= new List<CheckResults>();
    [ForeignKey("WebsiteId")]
    public int WebsiteId { get; set; } // Foreign key
    

    public Website Website { get; set; } // Navigation property (optional)
}