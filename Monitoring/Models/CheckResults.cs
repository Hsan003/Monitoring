using System.Runtime.InteropServices.JavaScript;

namespace Monitoring.Models;

public class CheckResults
{
    public int Id { get; set; }
    public DateTime CheckTime { get; set; }
    public string Status { get; set; }
    public string ResponseTime { get; set; }
    public string ErrorMessage { get; set; }
    
    public int AnalyticsId { get; set; }
    
    public virtual Analytics Analytics { get; set; }
    
}