namespace Monitoring.Models.MonitoringModule.checker;

public interface IChecker
{
    public  Task<CheckResult> check(Website website ); 
    
    public void initialize(Dictionary<string, object> param , MonitoringDbContext context);
}