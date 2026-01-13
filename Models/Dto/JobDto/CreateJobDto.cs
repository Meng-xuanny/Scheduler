using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Models.Dto
{
    public class CreateJobDto
    {
        [Required]
        public string ServiceRequestId { get; set; }
        public string ProviderName { get; set; }
        public string ServiceType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; }
        public string? Notes { get; set; }
        public bool IsClientConfirmed { get; set; } = false;
        public DateTime? ClientConfirmedAt { get; set; }
    }
}