using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Monitoring.Models;
using Monitoring.Models.MonitoringModule.checker;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Monitoring.Models.MonitoringModule;
using Monitoring.Repositories;
using Monitoring.Services;

namespace Monitoring.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class MonitoringController : ControllerBase
{
    private readonly MonitoringService _monitoringService;

    public MonitoringController(
        MonitoringService monitoringService)
    { 
        _monitoringService = monitoringService;
    }

    [HttpGet("websites")]
    public async Task<IEnumerable<Website>> GetWebsites()
    {
        return await _monitoringService.GetWebsites();
    }

    [HttpPost("check/{id}")]
    public async Task<ActionResult<Website>>RunCheck(int id)
    {
        try
        {
            var website = await _monitoringService.RunCheck(id, HttpContext);
            return Ok(website);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        
    }
    [HttpDelete("delete/{id}/{checkerId?}")]
    public void Delete(int id,int? checkerId = null)
    {
        _monitoringService.Delete(id,checkerId,HttpContext);
    }

    // GET: api/monitoring/results/{websiteId}
    // [HttpGet("results/{websiteId}")]
    // public async Task<ActionResult<IEnumerable<CheckResults>>> GetCheckResults(int websiteId)
    // {
    //     try
    //     {
    //         var results = await _context.CheckResults
    //             .Where(cr => cr.websiteId == websiteId)
    //             .OrderByDescending(cr => cr.Timestamp)
    //             .AsNoTracking()
    //             .ToListAsync();
    //
    //         return Ok(results);
    //     }
    //     catch (Exception ex)
    //     {
    //         return Problem(
    //             detail: $"Error retrieving results: {ex.Message}",
    //             statusCode: StatusCodes.Status500InternalServerError);
    //     }
    // }

    // GET: api/monitoring/status/{websiteId}
    [HttpGet("status/{websiteId}")]
    public  CheckResults GetCurrentStatus(int websiteId)
    {
        return _monitoringService.GetCurrentStatus(websiteId,HttpContext.Request.Query["checkerClass"].ToString());
    }
}