using System.Data;
using Monitoring.Models.MonitoringModule.checker;

namespace Monitoring.Models.MonitoringModule
{
    public class MonitoringSystem
    {
        public Scheduler Scheduler { get; }
        public MonitoringDbContext DbConnection { get; }
        public CheckerFactory CheckerFactory { get; }
    
        // Constructor injection of dependencies
        public MonitoringSystem(Scheduler scheduler, MonitoringDbContext dbConnection, CheckerFactory checkerFactory)
        {
            Scheduler = scheduler;
            DbConnection = dbConnection;
            CheckerFactory = checkerFactory;
        }
    
        public void addUrl(Website website, int interval, string checkerClass, string content, int retries)
        {
            Console.WriteLine("Adding url");
            MonitoringJob job = new MonitoringJob
            {
                websiteId = website,
                interval = interval,
                checker = CheckerFactory.CreateChecker(checkerClass)
            };
            job.checker.initialize(content, retries);
    
            checker_entity checkerEntity = new checker_entity
            {
                content = content,
                class_name = checkerClass,
                interval = interval,
                retries = retries,
                WebsiteId = website.Id,
            };
    
            Console.WriteLine("Adding checker entity");
            DbConnection.Checkers.Add(checkerEntity);
    
            Console.WriteLine("Saving changes");
            DbConnection.SaveChanges();
    
            Console.WriteLine("Scheduling job");
            Scheduler.ScheduleJob(job);
        }
        public async Task<CheckResults> GetStatus(Website website , string checkerClass)
        {
            var checker = CheckerFactory.CreateChecker(checkerClass);
            return await checker.check(website);
        }

        public void removeUrl(Website website)
        {
            foreach (var job in Scheduler.Jobs)
            {
                if (job.websiteId.Id == website.Id)
                {
                    Scheduler.StopJob(job);
                }
            }
        }

        public void StopJob(Website website, int interval, string checkerClass, string content, int retries)
        {
            MonitoringJob job = new MonitoringJob
            {
                websiteId = website,
                interval = interval,
                checker = CheckerFactory.CreateChecker(checkerClass)
            };
            job.checker.initialize(content, retries);
    
            Scheduler.StopJob(job);
        }
    
        public void startMonitoring()
        {
            Scheduler.Start();
        }
    
        public void stopMonitoring()
        {
            Scheduler.Stop();
        }
    }
}
