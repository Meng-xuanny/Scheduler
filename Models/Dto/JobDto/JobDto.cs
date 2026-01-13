using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Models.Dto.JobDto
{
    public class JobDto
    {
        public string Id { get; set; }
        public string ServiceRequestId { get; set; }
        public string ClientName { get; set; }
        public string ProviderName { get; set; }
        public string ServiceType { get; set; }
        public string Address { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; }
        public string? Notes { get; set; }
        public bool IsClientConfirmed { get; set; } 
        public DateTime? ClientConfirmedAt { get; set; }
    }
}