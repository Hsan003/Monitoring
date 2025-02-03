using System.Net;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Serialization;

namespace Monitoring.Models;

public class CheckResults
{
    public int Id { get; set; }
    public int? websiteId { get; set; }
    public HttpStatusCode? status { get; set; }
    
    public DateTime? Timestamp { get; set; }

    public Boolean? isUp { get; set; }

    public DateTime? CheckTime { get; set; }
    public int? ResponseTime { get; set; }
    public string? ErrorMessage { get; set; }
    
    public int? AnalyticsId { get; set; }
    [JsonIgnore] // 🔴 Prevents infinite loops during JSON serialization
    public virtual Analytics Analytics { get; set; }
    
    public string ToString()
    {
        return "CheckResult{" +
               "Id=" + Id +
               ", websiteId=" + websiteId +
               ", status=" + status +
               ", responseTime=" + ResponseTime +
               ", error='" + ErrorMessage + '\'' +
               ", isUp=" + isUp +
               ", Timestamp=" + Timestamp +
               '}';
    }
}