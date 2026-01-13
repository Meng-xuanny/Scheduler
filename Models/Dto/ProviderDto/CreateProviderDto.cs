using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Models.Dto
{
    public class CreateProviderDto
    {
        [Required]
        public string FullName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Skills { get; set; }

        public bool IsAvailable{ get; set; }= true;
    }
}
