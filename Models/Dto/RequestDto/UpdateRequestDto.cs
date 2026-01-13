using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Models.Dto.RequestDto
{
    public class UpdateRequestDto
    {
        [Required]
        public required string Id { get; set; }

        [Required]
        public required string ClientId { get; set; }

        [Required]
        public required string ServiceType { get; set; }

        [Required]
        public DateTime PreferredDate { get; set; }
        public string? ProviderId { get; set; }

        public string? Notes { get; set; }

        public string Status { get; set; } = "Pending";
    }
}
