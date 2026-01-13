using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scheduler.Data;
using Scheduler.Models;
using Scheduler.Models.Dto.ClientDto;

namespace clientApp.Controllers
{
    [ApiController]
    [Route("api/client")]
    public class ClientController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetAllClients()
        {
            return Ok(await _context.Client.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(string id)
        {
            var client = await _context.Client.FindAsync(id);
            if (client == null)
                return NotFound();

            return Ok(client);
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> LookupClient(
            [FromQuery] string email,
            string phone,
            string name
        )
        {
            var client = await _context.Client.FirstOrDefaultAsync(c =>
                c.Email == email && c.PhoneNumber == phone && c.FullName == name
            );

            if (client == null)
                return NotFound();

            return Ok(client);
        }

        [HttpPost]
        public async Task<ActionResult<Client>> CreateClient([FromBody] Client newClient)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            newClient.Id = Guid.NewGuid().ToString();

            _context.Client.Add(newClient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClient), new { id = newClient.Id }, newClient);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(string id, [FromBody] UpdateClientDto dto)
        {
            if (id != dto.Id)
                return BadRequest("Client ID mismatch.");

            var client = await _context.Client.FindAsync(id);
            if (client == null)
                return NotFound();

            // Update only non-null fields
            if (!string.IsNullOrWhiteSpace(dto.FullName))
                client.FullName = dto.FullName;
            if (dto.Age.HasValue)
                client.Age = dto.Age;
            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                client.PhoneNumber = dto.PhoneNumber;
            if (!string.IsNullOrWhiteSpace(dto.Email))
                client.Email = dto.Email;
            if (!string.IsNullOrWhiteSpace(dto.Address))
                client.Address = dto.Address;
            if (!string.IsNullOrWhiteSpace(dto.Work))
                client.Work = dto.Work;
            if (!string.IsNullOrWhiteSpace(dto.Hobby))
                client.Hobby = dto.Hobby;
            if (!string.IsNullOrWhiteSpace(dto.ServicePreference))
                client.ServicePreference = dto.ServicePreference;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpDto dto)
        {
            if (
                string.IsNullOrWhiteSpace(dto.Email)
                || string.IsNullOrWhiteSpace(dto.PhoneNumber)
                || string.IsNullOrWhiteSpace(dto.FullName)
            )
            {
                return BadRequest("Name, email, and phone number are required.");
            }

            var existing = await _context.Client.FirstOrDefaultAsync(c =>
                c.Email == dto.Email && c.PhoneNumber == dto.PhoneNumber
            );

            if (existing != null)
                return Conflict("Client already exists. Try signing in instead.");

            var client = new Client
            {
                Id = Guid.NewGuid().ToString(),
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FullName = dto.FullName,
                Address = dto.Address,
                ServicePreference = dto.ServicePreference,
            };

            _context.Client.Add(client);
            await _context.SaveChangesAsync();

            return Ok(client);
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] ClientLoginDto login)
        {
            var client = await _context.Client.FirstOrDefaultAsync(c =>
                c.Email == login.Email
                && c.PhoneNumber == login.PhoneNumber
                && c.FullName == login.FullName
            );

            if (client == null)
                return NotFound("No matching client found.");

            return Ok(client); // Optionally return a token/session later
        }
    }
}
