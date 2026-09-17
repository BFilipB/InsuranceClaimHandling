using Claims.Models;
using Claims.Services;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
{
    /// <summary>
    /// HTTP endpoints for creating, reading and deleting insurance Claims.
    /// All business logic lives in <see cref="IClaimsService"/> — this controller only
    /// translates HTTP requests into service calls and maps the results back to HTTP responses.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly IClaimsService _claimsService;
        private readonly ILogger<ClaimsController> _logger;

        public ClaimsController(IClaimsService claimsService, ILogger<ClaimsController> logger)
        {
            _claimsService = claimsService;
            _logger = logger;
        }

        /// <summary>Returns every claim.</summary>
        [HttpGet]
        public async Task<IEnumerable<Claim>> GetAsync()
        {
            return await _claimsService.GetAllAsync();
        }

        /// <summary>Returns a single claim by id, or 404 if it doesn't exist.</summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Claim>> GetAsync(string id)
        {
            var claim = await _claimsService.GetByIdAsync(id);
            return claim is null ? NotFound() : Ok(claim);
        }

        /// <summary>Creates a new claim. Returns 400 with error details if validation fails.</summary>
        [HttpPost]
        public async Task<ActionResult<Claim>> CreateAsync(Claim claim)
        {
            var created = await _claimsService.CreateAsync(claim);
            return Ok(created);
        }

        /// <summary>Deletes a claim by id.</summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            await _claimsService.DeleteAsync(id);
            return Ok();
        }
    }
}
