using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Monitoring.Models;
using Monitoring.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Monitoring.Services;

namespace Monitoring.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Client> _userManager;
        private readonly SignInManager<Client> _signInManager;
        private readonly IConfiguration _config;
        private readonly IEmailSender<Client> _emailSender;

        public AuthController(UserManager<Client> userManager, SignInManager<Client> signInManager, IConfiguration config, IEmailSender<Client> emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _emailSender = emailSender;
        }

        // ✅ Register User
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var user = new Client { UserName = model.Email, Email = model.Email, Name = model.Name, PhoneNumber = model.Phone };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Generate email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, token = token }, Request.Scheme);

            // Send confirmation email
            await _emailSender.SendConfirmationLinkAsync(user, user.Email, confirmationLink);
            
            await _userManager.AddToRoleAsync(user, "User");
            return Ok(new { message = "Registration successful. Please check your email to confirm your account." });
        }
        

        // ✅ Login User
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Invalid credentials");
            
            if (!await _userManager.IsEmailConfirmedAsync(user))
                return Unauthorized("Please confirm your email before logging in.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Invalid credentials");
            var userRoles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user,userRoles);
            return Ok(new { Token = token});
        }

        // ✅ Logout User
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logout successful" });
        }

        // ✅ JWT Token Generation
        private string GenerateJwtToken(Client user, IList<string> roles)
        { 
            string jwtKey = _config["Jwt:Key"];

            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new Exception("JWT Key is missing in configuration.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            // ✅ Add role claims to the JWT
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],     // Ensure issuer is set
                audience: _config["Jwt:Audience"], // Ensure audience is set
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        
        //Confirmation Email Method
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return BadRequest("Invalid user.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
                return Ok(new { message = "Email confirmed successfully" });
            else
                return BadRequest("Invalid or expired token.");
        }
        
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("Email not found.");

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            
            await _emailSender.SendPasswordResetCodeAsync(user, user.Email, resetToken);

            return Ok(new { message = "Password reset instructions have been sent to your email." });
        }


        
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (model.NewPassword != model.ConfirmPassword)
                return BadRequest("Passwords do not match.");

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return BadRequest("Invalid user ID.");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Password has been reset successfully." });
        }




        

    }
    }

