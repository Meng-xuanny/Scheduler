using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace Scheduler.Models
{
    public class ServiceRequest
    {
        [Key]
        public required string Id { get; set; }

        [Required]
        public string ClientId { get; set; }

        [ForeignKey("ClientId")]
        [JsonIgnore]
        public Client Client { get; set; }

        [Required]
        public required string ServiceType { get; set; }

        public string? ProviderId { get; set; }

        [ForeignKey("ProviderId")]
        public Provider? Provider { get; set; }


        public DateTime PreferredDate { get; set; }

        public string? Notes { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime RequestedAt { get; set; } = DateTime.Now;

        public bool IsQuote { get; set; } = false;
    }
}