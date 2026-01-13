using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Models.Dto
{
    public class UpdateJobDto
    {
        //  public string ClientName { get; set; }
        // public string ProviderName { get; set; }
        // public string ServiceType { get; set; }
        // public string Address { get; set; }
        // public DateTime StartTime { get; set; }
        // public DateTime EndTime { get; set; }
        public string Status { get; set; }
        public string? Notes { get; set; }
    }
}