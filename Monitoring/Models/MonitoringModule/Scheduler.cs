using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Monitoring.Models.MonitoringModule.checker;

namespace Monitoring.Models.MonitoringModule;

public sealed class Scheduler
{
    // Singleton setup
    private readonly IServiceProvider _serviceProvider;
    // Remove the static lazy singleton pattern if you want DI to control the lifetime
    public Scheduler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        Jobs = new List<MonitoringJob>();
    }
    public List<MonitoringJob> Jobs { get; private set; }
    private readonly Dictionary<MonitoringJob, Timer> jobTimers = new Dictionary<MonitoringJob, Timer>();
    private readonly object _lock = new object();

    // Schedule a job (thread-safe)
    public void ScheduleJob(MonitoringJob job)
    {
        Console.WriteLine("Scheduling job");
        lock (_lock)
        {
            if (Jobs.Contains(job)) return;
            Jobs.Add(job);
            var timer = new Timer(ExecuteJob, job, 0, job.interval);
            Console.WriteLine("Job scheduled");
            jobTimers.Add(job, timer);
        }
    }

    // Execute the job's check
    private async void ExecuteJob(object state)
    {
        if (state is MonitoringJob job)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MonitoringDbContext>();
                try
                {
                    Console.WriteLine("Starting check...");
                    CheckResults result = await job.runCheck();
                    Console.WriteLine($"Check result: {result}");
            
                    dbContext.CheckResults.Add(result);
                    Console.WriteLine("Saving to database...");
                    await dbContext.SaveChangesAsync();
                    Console.WriteLine("Save completed");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in job {job}: {ex.Message}");
                }
            }
        }
    }

    // Start all jobs
    public void Start()
    {
        lock (_lock)
        {
            foreach (var job in Jobs)
            {
                ScheduleJob(job);
            }
        }
    }

    // Stop all jobs
    public void Stop()
    {
        lock (_lock)
        {
            foreach (var timer in jobTimers.Values)
            {
                timer.Dispose();
            }
            jobTimers.Clear();
            Jobs.Clear();
        }
    }

    // Stop a specific job
    public void StopJob(MonitoringJob job)
    {
        lock (_lock)
        {
            if (jobTimers.TryGetValue(job, out var timer))
            {
                timer.Dispose();
                jobTimers.Remove(job);
                Jobs.Remove(job);
            }
        }
    }

    // Remove a job from the scheduler
    public void RemoveJob(MonitoringJob job)
    {
        lock (_lock)
        {
            if (Jobs.Remove(job))
            {
                if (jobTimers.TryGetValue(job, out var timer))
                {
                    timer.Dispose();
                    jobTimers.Remove(job);
                }
            }
        }
    }
}