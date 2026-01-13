using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Models.Dto.ClientDto
{
    public class CreateClientDto
    {
        [Required]
        public required string FullName { get; set; }

        public int? Age { get; set; }

        public required string PhoneNumber { get; set; }

        public required string Email { get; set; }

        public required string Address { get; set; }

        public string? Work { get; set; }

        public string? Hobby { get; set; }

        public string? FamilyInfo { get; set; }

        public string? ServicePreference { get; set; }

        public bool IsReferrer { get; set; }

        public string? ReferredByClientId { get; set; }
    }
}
