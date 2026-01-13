using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scheduler.Data;
using Scheduler.Models;
using Scheduler.Models.Dto.RequestDto;
using Scheduler.Models.Dtos.ServiceDto;

namespace Scheduler.Controllers
{
    [ApiController]
    [Route("api/request")]
    public class ServiceRequestController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServiceRequestController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceRequest>> CreateRequest(
            [FromBody] CreateRequestDto dto
        )
        {
            var request = new ServiceRequest
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = dto.ClientId,
                ProviderId = dto.ProviderId,
                ServiceType = dto.ServiceType,
                PreferredDate = DateTime.SpecifyKind(dto.PreferredDate, DateTimeKind.Local),
                Notes = dto.Notes,
                Status = "Pending",
                RequestedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local),
                IsQuote = dto.IsQuote,
            };

            _context.ServiceRequest.Add(request);
            await _context.SaveChangesAsync();

            // Fetch the client's full name
            var client = await _context.Client.FirstOrDefaultAsync(c => c.Id == request.ClientId);
            var clientName = client?.FullName ?? "Unknown Client";

            // Compose message based on whether it's a quote
            var messageType = request.IsQuote ? "Quote request" : "New service request";

            // Add provider notification
            var notification = new ProviderNotification
            {
                Id = Guid.NewGuid().ToString(),
                ServiceRequestId = request.Id,
                ProviderId = request.ProviderId,
                Message =
                    $"{messageType} from {clientName}: {request.ServiceType} on {request.PreferredDate:g}",
                CreatedAt = DateTime.UtcNow,
                Status = "Unread",
            };

            _context.ProviderNotification.Add(notification);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetRequest),
                new { id = request.Id },
                new ServiceRequestDto
                {
                    Id = request.Id,
                    ClientId = request.ClientId,
                    ProviderId = request.ProviderId,
                    ServiceType = request.ServiceType,
                    PreferredDate = request.PreferredDate,
                    Notes = request.Notes,
                    Status = request.Status,
                    RequestedAt = request.RequestedAt,
                    IsQuote = request.IsQuote,
                }
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRequest(string id, [FromBody] UpdateRequestDto dto)
        {
            if (id != dto.Id)
                return BadRequest("Request ID mismatch.");

            var request = await _context.ServiceRequest.FindAsync(id);
            if (request == null)
                return NotFound();

            // Detect changes
            bool notifyProvider =
                request.ServiceType != dto.ServiceType
                || request.PreferredDate != dto.PreferredDate;

            // Apply updates
            request.ServiceType = dto.ServiceType;
            request.PreferredDate = dto.PreferredDate;
            request.Notes = dto.Notes;
            request.Status = dto.Status;
            request.ProviderId = dto.ProviderId;

            await _context.SaveChangesAsync();

            if (notifyProvider)
            {
                var client = await _context.Client.FirstOrDefaultAsync(c =>
                    c.Id == request.ClientId
                );
                var clientName = client?.FullName ?? "Unknown Client";

                var messageType = request.IsQuote ? "Quote request" : "New service request";

                var notification = new ProviderNotification
                {
                    Id = Guid.NewGuid().ToString(),
                    ServiceRequestId = request.Id,
                    ProviderId = request.ProviderId,
                    Message =
                        $"{messageType} from {clientName}: {request.ServiceType} on {request.PreferredDate:g}",
                    CreatedAt = DateTime.UtcNow,
                    Status = "Unread",
                };

                _context.ProviderNotification.Add(notification);
                await _context.SaveChangesAsync();
            }
            return NoContent();
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateRequestStatus(
            string id,
            [FromBody] StatusUpdateDto dto
        )
        {
            var request = await _context.ServiceRequest.FindAsync(id);
            if (request == null)
                return NotFound();

            request.Status = dto.Status;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetAllRequests()
        {
            var requests = await _context
                .ServiceRequest.Include(sr => sr.Client)
                .Include(sr => sr.Provider)
                .Select(sr => new ServiceRequestDto
                {
                    Id = sr.Id,
                    ClientId = sr.ClientId,
                    ClientName = sr.Client.FullName ?? "Unknown",
                    ProviderId = sr.ProviderId,
                    ProviderName = sr.Provider != null ? sr.Provider.FullName : "Unassigned",
                    ServiceType = sr.ServiceType,
                    PreferredDate = sr.PreferredDate,
                    Notes = sr.Notes,
                    Status = sr.Status,
                    RequestedAt = sr.RequestedAt,
                    IsQuote = sr.IsQuote,
                })
                .ToListAsync();

            return Ok(requests);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceRequest>> GetRequest(string id)
        {
            var req = await _context.ServiceRequest.FindAsync(id);
            if (req == null)
                return NotFound();

            return Ok(req);
        }

        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<IEnumerable<ServiceRequest>>> GetRequestsForClient(
            string clientId
        )
        {
            var requests = await _context
                .ServiceRequest.Where(r => r.ClientId == clientId)
                .ToListAsync();

            return Ok(requests);
        }
    }
}
