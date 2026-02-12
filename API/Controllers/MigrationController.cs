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
        public async Task<List<MigrationReport>> Search([FromQuery] MigrationReport filter)
        {
            return await _migrationService.SearchAsync(filter);
        }

        [HttpPost("check")]
        public async Task<List<string>> Check([FromBody] List<string> hashes)
        {
            return await _migrationService.GetExistingHashesAsync(hashes);
        }
    }
}
