using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scheduler.Data;
using Scheduler.Models;

namespace Scheduler.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/feedback")]
    public class FeedbackController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FeedbackController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Feedback>> GetById(string id)
        {
            var feedback = await _context.Feedback.FindAsync(id);
            if (feedback == null)
                return NotFound();
            return Ok(feedback);
        }

        [HttpGet("job/{JobId}")]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetByScheduledService(
            string scheduledServiceId
        )
        {
            var feedbacks = await _context
                .Feedback.Where(f => f.JobId == scheduledServiceId)
                .ToListAsync();
            return Ok(feedbacks);
        }

        [HttpPost]
        public async Task<ActionResult<Feedback>> Create([FromBody] Feedback feedback)
        {
            feedback.Id = Guid.NewGuid().ToString();
            feedback.CreatedAt = DateTime.UtcNow;
            _context.Feedback.Add(feedback);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = feedback.Id }, feedback);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var feedback = await _context.Feedback.FindAsync(id);
            if (feedback == null)
                return NotFound();

            _context.Feedback.Remove(feedback);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
