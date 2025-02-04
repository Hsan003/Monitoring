using Microsoft.AspNetCore.Mvc;
using Monitoring.Models;
using Monitoring.Models.DashboardModule;
using Monitoring.Services;

namespace Monitoring.Controllers;
[Route("Dashboard")]
[ApiController]
public class DashboardController:Controller
{  
   private readonly AnalyticsService AnalyticsService;
   public DashboardController(AnalyticsService analyticsService) // ✅ Injected via constructor
   {
       AnalyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));
   }

   [HttpGet()]
   public async Task<IEnumerable<CheckResults>> GetAnalytics([FromQuery] GetAnalyticsRequest request)
   {
       if (request.AnalyticsId==0)
           return Array.Empty<CheckResults>();

       var results = await AnalyticsService.GetAnalytics(request);
       return results;
   }

   

}