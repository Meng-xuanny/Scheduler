using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scheduler.Data;
using Scheduler.Models;
using Scheduler.Models.Dto;
using Scheduler.Models.Dto.JobDto;

namespace Scheduler.Controllers
{
    [Route("api/job")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public JobsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetJobs()
        {
            try
            {
                var jobs = await _context
                    .Job.Include(j => j.ServiceRequest)
                    .ThenInclude(sr => sr.Client)
                    .Include(j => j.ServiceRequest)
                    .ThenInclude(sr => sr.Provider)
                    .Select(j => new JobDto
                    {
                        Id = j.Id,
                        ServiceRequestId = j.ServiceRequestId,
                        ClientName = j.ServiceRequest.Client.FullName ?? "Unknown",
                        ProviderName =
                            j.ProviderName
                            ?? (
                                j.ServiceRequest.Provider != null
                                    ? j.ServiceRequest.Provider.FullName
                                    : "Unassigned"
                            ),
                        ServiceType = j.ServiceType,
                        Address = j.Address,
                        StartTime = j.StartTime,
                        EndTime = j.EndTime,
                        Status = j.Status,
                        Notes = j.Notes,
                        IsClientConfirmed = j.IsClientConfirmed,
                        ClientConfirmedAt = j.ClientConfirmedAt,
                    })
                    .ToListAsync();

                return Ok(jobs);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching jobs: " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetJobsByClient(string clientId)
        {
            try
            {
                var jobs = await _context
                    .Job.Where(j => j.ServiceRequestId != null)
                    .Join(
                        _context.ServiceRequest,
                        job => job.ServiceRequestId,
                        sr => sr.Id,
                        (job, sr) => new { job, sr }
                    )
                    .Where(joined => joined.sr.ClientId == clientId)
                    .Select(joined => joined.job)
                    .ToListAsync();

                return Ok(jobs);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching jobs for client: " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateJob([FromBody] CreateJobDto dto)
        {
            // Fetch the related ServiceRequest and include Client
            var serviceRequest = await _context
                .ServiceRequest.Include(sr => sr.Client)
                .FirstOrDefaultAsync(sr => sr.Id == dto.ServiceRequestId);

            if (serviceRequest == null)
                return BadRequest("Invalid ServiceRequest ID.");

            var job = new Job
            {
                Id = Guid.NewGuid().ToString(),
                ServiceRequestId = dto.ServiceRequestId,
                ClientName = serviceRequest.Client?.FullName ?? "Unknown",
                ProviderName = dto.ProviderName ?? "Unassigned",
                ServiceType = dto.ServiceType,
                Address = serviceRequest.Client?.Address ?? "TBD",
                StartTime = dto.StartTime,
                EndTime = dto.EndTime ?? null,
                Status = dto.Status,
                Notes = dto.Notes,
                IsClientConfirmed = dto.IsClientConfirmed,
                ClientConfirmedAt = dto.ClientConfirmedAt,
            };

            _context.Job.Add(job);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetJobs), new { id = job.Id }, job);
        }

        // [HttpPut("{id}")]
        // public async Task<IActionResult> UpdateJob(string id, [FromBody] UpdateJobDto dto)
        // {
        //     var job = await _context.Job.FindAsync(id);
        //     if (job == null)
        //         return NotFound();

        //     job.ClientName = dto.ClientName;
        //     job.ProviderName = dto.ProviderName;
        //     job.ServiceType = dto.ServiceType;
        //     job.Address = dto.Address;
        //     job.StartTime = dto.StartTime;
        //     job.EndTime = dto.EndTime;
        //     job.Status = dto.Status;
        //     job.Notes = dto.Notes;

        //     await _context.SaveChangesAsync();
        //     return NoContent();
        // }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateJobStatus(string id, [FromBody] UpdateJobDto dto)
        {
            var job = await _context.Job.FindAsync(id);
            if (job == null)
                return NotFound();

            job.Status = dto.Status;
            if (!string.IsNullOrWhiteSpace(dto.Notes))
                job.Notes = dto.Notes;
            await _context.SaveChangesAsync();

            return Ok(job);
        }

        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> ConfirmJob(string id)
        {
            var job = await _context.Job.FindAsync(id);
            if (job == null)
                return NotFound("Job not found.");

            job.IsClientConfirmed = true;
            job.ClientConfirmedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Job confirmed by client." });
        }

        [HttpPut("{id}/complete")]
        public async Task<IActionResult> MarkJobCompleted(string id)
        {
            var job = await _context.Job.FindAsync(id);
            if (job == null)
                return NotFound();

            job.EndTime = DateTime.Now;
            job.Status = "Completed";

            await _context.SaveChangesAsync();
            return Ok(job);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(string id)
        {
            var job = await _context.Job.FindAsync(id);
            if (job == null)
                return NotFound();

            _context.Job.Remove(job);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
