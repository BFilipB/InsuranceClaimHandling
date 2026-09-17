using Claims.Models;
using Claims.Services;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
{
    /// <summary>
    /// HTTP endpoints for creating, reading and deleting insurance Covers, and for
    /// computing a premium quote without creating a Cover.
    /// All business logic lives in <see cref="ICoversService"/>.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class CoversController : ControllerBase
    {
        private readonly ICoversService _coversService;
        private readonly ILogger<CoversController> _logger;

        public CoversController(ICoversService coversService, ILogger<CoversController> logger)
        {
            _coversService = coversService;
            _logger = logger;
        }

        /// <summary>Computes what the premium would be for the given parameters, without creating a Cover.</summary>
        [HttpPost("compute")]
        public ActionResult<decimal> ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            return Ok(_coversService.ComputePremium(startDate, endDate, coverType));
        }

        /// <summary>Returns every cover.</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
        {
            return Ok(await _coversService.GetAllAsync());
        }

        /// <summary>Returns a single cover by id, or 404 if it doesn't exist.</summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Cover>> GetAsync(string id)
        {
            var cover = await _coversService.GetByIdAsync(id);
            return cover is null ? NotFound() : Ok(cover);
        }

        /// <summary>Creates a new cover; the premium is computed server-side. Returns 400 if validation fails.</summary>
        [HttpPost]
        public async Task<ActionResult<Cover>> CreateAsync(Cover cover)
        {
            var created = await _coversService.CreateAsync(cover);
            return Ok(created);
        }

        /// <summary>Deletes a cover by id.</summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            await _coversService.DeleteAsync(id);
            return Ok();
        }
    }
}
