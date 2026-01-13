using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Models.Dto.RequestDto
{
    public class ServiceRequestDto
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; } 
        public string ProviderId { get; set; }
        public string ProviderName { get; set; } 
        public string ServiceType { get; set; }
        public DateTime PreferredDate { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public bool IsQuote { get; set; }
    
    }
}