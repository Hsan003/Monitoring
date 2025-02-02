using Microsoft.AspNetCore.Mvc;
using Monitoring.Models;
using Monitoring.Models.DashboardModule;

namespace Monitoring.Services;

public class AnalyticsService
{private readonly CheckResultsRepository checkrepo;

    public AnalyticsService(CheckResultsRepository repository)
    {
        checkrepo = repository;
    }

    public async Task<IEnumerable<CheckResults>> GetAnalytics(GetAnalyticsRequest request)
    {
        return await checkrepo.GetByAnalyticsAndTimeRangeAsync(request);
    }

}