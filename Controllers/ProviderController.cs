using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scheduler.Data;
using Scheduler.Models;
using Scheduler.Models.Dto;

namespace Scheduler.Controllers
{
    [ApiController]
    [Route("api/provider")]
    public class ProviderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProviderController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/provider
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Provider>>> GetProviders()
        {
            var providers = await _context.Provider.ToListAsync();
            return Ok(providers);
        }

        // GET: api/provider/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Provider>> GetProvider(string id)
        {
            var provider = await _context.Provider.FindAsync(id);
            if (provider == null)
                return NotFound();

            return Ok(provider);
        }

        // POST: api/provider
        [HttpPost]
        public async Task<ActionResult<Provider>> CreateProvider([FromBody] CreateProviderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var provider = new Provider
            {
                Id = Guid.NewGuid().ToString(),
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Skills = dto.Skills,
                IsAvailable = dto.IsAvailable,
            };

            _context.Provider.Add(provider);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProvider), new { id = provider.Id }, provider);
        }
    }
}
