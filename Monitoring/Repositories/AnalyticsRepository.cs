using Microsoft.EntityFrameworkCore;
using Monitoring.Models;

public class AnalyticsRepository 
{
    private readonly MonitoringDbContext _context;

    public AnalyticsRepository(MonitoringDbContext context)
    {   
        _context = context;
    }

    public async Task<IEnumerable<Analytics>> GetAllAsync()
    {
        return await _context.Analytics.Include(a => a.CheckResults).ToListAsync();
    }

    public async Task<Analytics> GetByIdAsync(int id)
    {
        return await _context.Analytics.Include(a => a.CheckResults)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task AddAsync(Analytics analytics)
    {
        await _context.Analytics.AddAsync(analytics);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Analytics analytics)
    {
        _context.Analytics.Update(analytics);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var analytics = await _context.Analytics.FindAsync(id);
        if (analytics != null)
        {
            _context.Analytics.Remove(analytics);
            await _context.SaveChangesAsync();
        }
    }
}