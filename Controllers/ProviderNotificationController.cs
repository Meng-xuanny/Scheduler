using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scheduler.Data;
using Scheduler.Models;

namespace Scheduler.Controllers
{
    [ApiController]
    [Route("api/notification")]
    public class ProviderNotificationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProviderNotificationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var notifications = await _context
                .ProviderNotification.Include(n => n.ServiceRequest)
                .ThenInclude(sr => sr.Client)
                .Select(n => new
                {
                    n.Id,
                    n.ServiceRequestId,
                    n.Message,
                    n.CreatedAt,
                    ClientPhone = n.ServiceRequest.Client.PhoneNumber,
                    ClientId = n.ServiceRequest.ClientId,
                    ServiceType = n.ServiceRequest.ServiceType,
                    PreferredDate = n.ServiceRequest.PreferredDate,
                    ProviderId = n.ServiceRequest.ProviderId,
                    Notes = n.ServiceRequest.Notes,
                    Status = n.Status,
                })
                .ToListAsync();

            return Ok(notifications);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var notification = await _context.ProviderNotification.FindAsync(id);
            if (notification == null)
                return NotFound();

            return Ok(notification);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] string status)
        {
            var notification = await _context.ProviderNotification.FindAsync(id);
            if (notification == null) return NotFound();

            notification.Status = status;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var notification = await _context.ProviderNotification.FindAsync(id);
            if (notification == null)
                return NotFound();

            _context.ProviderNotification.Remove(notification);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
