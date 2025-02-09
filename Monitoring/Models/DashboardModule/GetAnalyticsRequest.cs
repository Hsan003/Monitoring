using System.Net;
using System.Runtime.InteropServices.JavaScript;

namespace Monitoring.Models.DashboardModule;

public class GetAnalyticsRequest
{
    public int AnalyticsId { get; set; } = 0;
    public DateTime startDate { get; set;}= Convert.ToDateTime("2000-01-01") ;
    public DateTime endDate { get; set;}=DateTime.Now;
    public string Status { get; set; } = "";
    public int ResponseTime { get; set; } = -1;
    public string ErrorMessage { get; set; } = "";

}