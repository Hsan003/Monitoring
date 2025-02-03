using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace Monitoring.Models.MonitoringModule.checker;

public class CheckResult
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int websiteId { get; set; }
    public virtual Website website { get; set; }
    public HttpStatusCode status { get; set; }
    public int responseTime { get; set; }
    public string error { get; set; }
    public Boolean isUp { get; set; }
    public DateTime Timestamp { get; set; }
    
    public string ToString()
    {
        return "CheckResult{" +
               "Id=" + Id +
               ", websiteId=" + websiteId +
               ", status=" + status +
               ", responseTime=" + responseTime +
               ", error='" + error + '\'' +
               ", isUp=" + isUp +
               ", Timestamp=" + Timestamp +
               '}';
    }
}