using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Models
{
    public class Client
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public required string FullName { get; set; }

        public int? Age { get; set; }

        public required string PhoneNumber { get; set; }

        public required string Email { get; set; }

        public string? Address { get; set; }

        public string? Work { get; set; }

        public string? Hobby { get; set; }

        public string? FamilyInfo { get; set; }

        public string? ServicePreference { get; set; }

        public bool IsReferrer { get; set; } = false;

        public string? ReferredByClientId { get; set; }

        [ForeignKey("ReferredByClientId")]
        public Client? ReferredBy { get; set; }
    }
}
