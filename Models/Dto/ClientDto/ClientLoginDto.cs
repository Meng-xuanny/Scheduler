using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Models.Dto.ClientDto
{
    public class ClientLoginDto
    {
        public required string FullName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }
    }
}