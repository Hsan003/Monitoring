namespace Monitoring.Models.MonitoringModule;

public class Scheduler
{
    public List<MonitoringJob> jobs { get; set; }
    private readonly Dictionary<MonitoringJob, Timer> jobTimers = new Dictionary<MonitoringJob, Timer>();

    public void scheduleJob(MonitoringJob job)
    {
        jobs.Add(job);
        var timer = new Timer(new TimerCallback(exec), job, 0, job.interval);
        jobTimers.Add(job, timer);
        
    }

    private void exec(object state)
    {
        if (state is MonitoringJob job)
        {
            Task.Run(async () => await job.runCheck());
        }
    }
    public void start()
    {
        foreach (var job in jobs)
        {
            scheduleJob(job);
        }
    }
    public void stop()
    {
        foreach (var timer in jobTimers.Values)
        {
            timer.Dispose();
        }
    }
    public void stopJob(MonitoringJob job)
    {
        if (jobTimers.TryGetValue(job, out var timer))
        {
            timer.Dispose();
        }
    }
    public void removeJob(MonitoringJob job)
    {
        if (jobs.Remove(job))
        {
            stopJob(job);
            jobTimers.Remove(job);
        }
        else
        {
            Console.WriteLine($"Job not found");
        }
    }
}