

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Monitoring.Models.MonitoringModule.checker;

namespace Monitoring.Models;
public class Website
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Url { get; set; }

    public string Status { get; set; }

    [ForeignKey("Client")]
    public int ClientId { get; set; }

    public virtual Client Client { get; set; }
    
    public Analytics Analytics { get; set; }
    
    public List<CheckResults> CheckResults { get; set; }
}

