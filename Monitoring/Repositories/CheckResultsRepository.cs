using Microsoft.EntityFrameworkCore;
using Monitoring.Models;
using Monitoring.Models.DashboardModule;

public class CheckResultsRepository 
{
    private readonly MonitoringDbContext _context;

    public CheckResultsRepository(MonitoringDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CheckResults>> GetAllAsync()
    {
        return await _context.CheckResults.Include(c => c.Analytics).ToListAsync();
    }

    public async Task<CheckResults> GetByIdAsync(int id)
    {
        return await _context.CheckResults.Include(c => c.Analytics)
                                          .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<CheckResults>> GetByTimeRangeAsync(DateTime startTime, DateTime endTime)
    {
        return await _context.CheckResults
            .Where(c => c.CheckTime >= startTime && c.CheckTime <= endTime)
            .Include(c => c.Analytics)
            .ToListAsync();
    }

    public async Task<IEnumerable<CheckResults>> GetByAnalyticsAndTimeRangeAsync(GetAnalyticsRequest request)
    {
        var query = _context.CheckResults.AsQueryable(); // Start with base query

        // Apply filters conditionally
        if (request.AnalyticsId != 0) // Only filter if WebsiteId is specified
        {
            query = query.Where(c => c.Analytics.WebsiteId == request.AnalyticsId);
        }

        if (request.startDate != Convert.ToDateTime("2000-01-01")) // Ignore if default value
        {
            query = query.Where(c => c.CheckTime >= request.startDate);
        }

        if (request.endDate != DateTime.Now) // Ignore if default value
        {
            query = query.Where(c => c.CheckTime <= request.endDate);
        }

        if (!string.IsNullOrEmpty(request.Status)) // Ignore if empty
        {
            query = query.Where(c => c.Status == request.Status);
        }

        if (!string.IsNullOrEmpty(request.ResponseTime)) // Ignore if empty
        {
            query = query.Where(c => c.ResponseTime == request.ResponseTime);
        }

        if (!string.IsNullOrEmpty(request.ErrorMessage)) // Ignore if empty
        {
            query = query.Where(c => c.ErrorMessage == request.ErrorMessage);
        }

        return await query.ToListAsync(); // Execute query
    }


    public async Task AddAsync(CheckResults checkResult)
    {
        await _context.CheckResults.AddAsync(checkResult);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CheckResults checkResult)
    {
        _context.CheckResults.Update(checkResult);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var checkResult = await _context.CheckResults.FindAsync(id);
        if (checkResult != null)
        {
            _context.CheckResults.Remove(checkResult);
            await _context.SaveChangesAsync();
        }
    }
}
