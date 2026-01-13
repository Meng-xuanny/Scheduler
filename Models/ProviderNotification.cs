using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Models
{
    public class ProviderNotification
    {
        [Key]
        public required string Id { get; set; }

        public string? ProviderId { get; set; }

        [ForeignKey("ProviderId")]
        public Provider? Provider { get; set; }

        public required string ServiceRequestId { get; set; }

        [ForeignKey("ServiceRequestId")]
        public ServiceRequest ServiceRequest { get; set; }

        public required string Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Unread";
    }
}
