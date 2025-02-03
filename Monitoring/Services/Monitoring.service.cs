using Monitoring.Models;
using Monitoring.Models.MonitoringModule;
using Monitoring.Models.MonitoringModule.checker;
using Monitoring.Repositories;

namespace Monitoring.Services;

public class MonitoringService
{
    private readonly IRepository<Website> _websiteRepository;
    private readonly MonitoringSystem _monitoringSystem;
    private readonly IRepository<checker_entity> _checkerRepository;
    public MonitoringService(IRepository<Website> websiteRepository,
        MonitoringSystem monitoringSystem,
        IRepository<checker_entity> checkerRepository)
    {
        _monitoringSystem = monitoringSystem;
        _websiteRepository = websiteRepository;        
        _checkerRepository = checkerRepository;
    }

    public async Task<IEnumerable<Website>> GetWebsites()
    {
        return await _websiteRepository.GetAllAsync();
    }

    public async Task<Website> RunCheck(int id,HttpContext httpContext)
    {
        var queryParams = httpContext.Request.Query;
        try
        {
            var website = await _websiteRepository.GetByIdAsync(id);

            if (website == null)
            {
                throw new ArgumentException($"Website with ID {id} not found");
            }

            _monitoringSystem.addUrl(website,
                int.Parse(queryParams["interval"].ToString()),
                queryParams["checkerClass"].ToString(),
                queryParams["content"].ToString(),
                int.Parse(queryParams["retries"].ToString()));

            return website;
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException(ex.Message);
        }
    }
    public void Delete(int id, int? checkerId,HttpContext httpContext)
    {
        var website = _websiteRepository.GetByIdAsync(id).Result;
        if (checkerId == null)
        {
            _monitoringSystem.removeUrl(website);
        }
        else
        {
            var param = httpContext.Request.Query;
            _monitoringSystem.StopJob(website,
                Int32.Parse(param["interval"].ToString()),
                param["checkerClass"].ToString(),
                param["content"].ToString(),
                Int32.Parse(param["retries"].ToString()));
            _checkerRepository.Delete((int)checkerId);
            
        }
    }
    public async Task<CheckResults> GetCurrentStatus(int websiteId,string checkerClass)
    {
        var website = await _websiteRepository.GetByIdAsync(websiteId);
        var res = await _monitoringSystem.GetStatus(website,checkerClass);
        return res;

    }
        
}