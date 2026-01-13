using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Models.Dto.ClientDto
{
    public class SignUpDto
    {
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string FullName { get; set; }
        public string? Address { get; set; }
        public string? ServicePreference { get; set; }
    }  
}