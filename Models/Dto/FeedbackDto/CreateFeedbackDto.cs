using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Models.Dto.FeedbackDto
{
    public class CreateFeedbackDto
    {
        [Required]
        public required string ScheduledServiceId { get; set; }

        [Required]
        public int Rating { get; set; }

        public string? Comments { get; set; }
    }
}
