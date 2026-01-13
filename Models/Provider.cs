using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Models
{
    public class Provider
    {
        [Key]
        public required string Id { get; set; }

        [Required]
        public required string FullName { get; set; }

        [Required]
        public required string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        public string? Address { get; set; }

        public string? Bio { get; set; }

        public string? Skills { get; set; }

        public bool IsAvailable { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation: A provider can have many service requests
        public ICollection<ServiceRequest>? ServiceRequests { get; set; }
    }
}
