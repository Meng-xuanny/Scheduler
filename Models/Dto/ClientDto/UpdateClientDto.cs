using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Models.Dto.ClientDto
{
    public class UpdateClientDto
    {
        [Required]
        public required string Id { get; set; }
        public string? FullName { get; set; }

        public int? Age { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public string? Work { get; set; }

        public string? Hobby { get; set; }
        public string? ServicePreference { get; set; }

    }
}
