

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitoring.Models;
public class Website
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Url { get; set; }

    public string Status { get; set; }

    [ForeignKey("Client")]
    public int ClientId { get; set; }

    public virtual Client Client { get; set; }
    
}

