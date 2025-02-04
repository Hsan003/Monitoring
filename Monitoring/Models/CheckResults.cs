using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Serialization;

namespace Monitoring.Models;

public class CheckResults
{
    public int Id { get; set; }
    public DateTime CheckTime { get; set; }
    public string Status { get; set; }
    public string ResponseTime { get; set; }
    public string ErrorMessage { get; set; }
    
    public int AnalyticsId { get; set; }
    [JsonIgnore] // 🔴 Prevents infinite loops during JSON serialization
    public virtual Analytics Analytics { get; set; }
    
}