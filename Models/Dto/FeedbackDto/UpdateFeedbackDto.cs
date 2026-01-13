using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Models.Dto.FeedbackDto
{
    public class UpdateFeedbackDto : CreateFeedbackDto
    {
        [Required]
        public required string Id { get; set; }
    }
}
