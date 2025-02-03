namespace Monitoring.Models.MonitoringModule.checker;

public interface IChecker
{
    public  Task<CheckResults> check(Website website ); 
    
    public void initialize(string content, int retries, MonitoringDbContext context);
}