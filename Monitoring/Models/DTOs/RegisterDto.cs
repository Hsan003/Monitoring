using System.ComponentModel.DataAnnotations;

namespace Monitoring.Models.DTOs
{
    public class RegisterDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }

        [Required(ErrorMessage = "At least one notification preference is required.")]
        public List<string> NotificationChannels { get; set; } // ["Email", "SMS"]
    }
}