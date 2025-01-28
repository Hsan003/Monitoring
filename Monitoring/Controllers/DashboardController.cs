using Microsoft.AspNetCore.Mvc;
using Monitoring.Models.DashboardModule;
using Monitoring.Services;

namespace Monitoring.Controllers;

public class DashboardController:Controller
{  
    private readonly AnalyticsService AnalyticsService;
    [HttpGet]
    public IActionResult GetAnalytics([FromQuery] GetAnalyticsRequest request)
    {
        if (request == null || request.WebsiteId <= 0)
        {
            return BadRequest("Invalid WebsiteId.");
        }
        return AnalyticsService.GetAnalytics(request);
    }
}