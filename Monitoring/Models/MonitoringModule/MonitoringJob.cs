using Monitoring.Models.MonitoringModule.checker;

namespace Monitoring.Models.MonitoringModule;

public class MonitoringJob
{
    public Website websiteId { get; set; }
    public int interval { get; set; }
    public IChecker checker { get; set; }

    public async Task<CheckResult> runCheck()
    {
        return await checker.check(websiteId);
    }
    
    
}