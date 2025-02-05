using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Monitoring.Models;

namespace Monitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InputController : ControllerBase
    {
        private readonly UserManager<Client> _userManager;
        private readonly MonitoringDbContext _context;
        public InputController(UserManager<Client> userManager, MonitoringDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Endpoint to add a website URL
        [HttpPost("addWebsite")]
        public async Task<IActionResult> AddWebsite([FromBody] string url)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized("User not found");

            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("Authentication failed");
            }

            // Check if the website already exists for the logged-in user
            var existingWebsite = _context.Websites
                .FirstOrDefault(w => w.ClientId == user.Id && w.Url == url);

            if (existingWebsite != null)
            {
                return BadRequest(new { message = "Website already added" });
            }

            var website = new Website
            {
                Url = url,
                ClientId = user.Id,
                Status = "Active"  
            };

            // Add the website to the database
            _context.Websites.Add(website);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Website added successfully" });
        }


        // Endpoint to get all websites for the logged-in user
        [HttpGet("websites")]
        public async Task<IActionResult> GetWebsites()
        {
            var websites = await _context.Websites
                .AsNoTracking()
                .Select(w => new 
                {
                    w.Id,
                    w.Url,
                    w.Status,
                    ClientId = w.Client.Id 
                })
                .ToListAsync();

            return Ok(websites);
        }
    }
}