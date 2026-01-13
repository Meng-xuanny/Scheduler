using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Models
{
    [Table("Job")]
    public class Job
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string ServiceRequestId { get; set; }

        public string ClientName { get; set; }
        public string ProviderName { get; set; }
        public string ServiceType { get; set; }
        public string Address { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; } = "Pending";
        public string? Notes { get; set; }
        public bool IsClientConfirmed { get; set; }
        public DateTime? ClientConfirmedAt { get; set; }

        [ForeignKey("ServiceRequestId")]
        public ServiceRequest ServiceRequest { get; set; }
    }
}
