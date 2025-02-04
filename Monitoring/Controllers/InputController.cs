using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Monitoring.Models;

namespace Monitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            var website = new Website
            {
                Url = url,
                ClientId = user.Id,
                Status = "Active"  // Default status or you can leave it blank to set later
            };

            // Add the website to the database
            _context.Websites.Add(website);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Website added successfully" });
        }

        // Endpoint to get all websites for the logged-in user
        [HttpGet("getWebsites")]
        public async Task<IActionResult> GetWebsites()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized("User not found");

            var websites = _context.Websites.Where(w => w.ClientId == user.Id).ToList();
            return Ok(websites);
        }
    }
}