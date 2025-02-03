using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitoring.Models.MonitoringModule.checker;

public class checker_entity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string class_name { get; set; }
    
    [ForeignKey("Website")] public int WebsiteId { get; set; }
    
    public string content { get; set; }
    public int retries { get; set; }
    public int interval { get; set; }
}