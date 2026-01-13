using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Scheduler.Models.Dtos.ServiceDto
{
    public class CreateRequestDto
    {
        [Required]
        public string ClientId { get; set; }

        public string ProviderId { get; set; }

        [Required]
        public string ServiceType { get; set; }

        [Required]
        public DateTime PreferredDate { get; set; }

        public string? Notes { get; set; }

        public bool IsQuote { get; set; }
    
    }
}