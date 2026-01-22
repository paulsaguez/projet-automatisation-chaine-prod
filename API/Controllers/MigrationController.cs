using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MigrationController : ControllerBase
    {
        private readonly MigrationService _migrationService;

        public MigrationController(MigrationService migrationService)
        {
            _migrationService = migrationService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] List<MigrationReport> reports)
        {
            await _migrationService.CreateManyAsync(reports);
            return CreatedAtAction(nameof(Post), new { count = reports.Count });
        }

        [HttpGet("search")]
        public async Task<List<MigrationReport>> Search([FromQuery] string? title, [FromQuery] string? status)
        {
            return await _migrationService.SearchAsync(title, status);
        }
    }
}
