using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduler.Models
{
    public class Feedback
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string JobId { get; set; }

        public int Rating { get; set; } // e.g., 1–5 stars

        public string Comments { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Job Job { get; set; }
    }
}
