using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Monitoring.Models;

namespace Monitoring.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<Client> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<Client> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // ✅ Create Role
        [HttpPost("create")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(new { message = "Role created" });
        }

        // ✅ Assign Role to User
        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User not found");

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(new { message = "Role assigned" });
        }
    }
}