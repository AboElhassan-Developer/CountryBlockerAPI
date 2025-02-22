using CountryBlockerAPI.Models;
using CountryBlockerAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CountryBlockerAPI.Controllers
{
    [Route("api/countries")]
    [ApiController]
    public class BlockController : Controller
    {
        private readonly BlockService _blockService;

        public BlockController(BlockService blockService)
        {
            _blockService = blockService;
        }

        //  Add a country to the block list
        [HttpPost("block")]
        public IActionResult BlockCountry([FromBody] CountryBlock request)
        {
            if (string.IsNullOrWhiteSpace(request.CountryCode))
                return BadRequest("Invalid country code.");

            bool isAdded = _blockService.BlockCountry(request.CountryCode);
            if (!isAdded)
                return Conflict($"Country {request.CountryCode} is already blocked.");

            return Ok($"Country {request.CountryCode} has been blocked.");
        }

        //  Remove a country from the block list
        [HttpDelete("block/{countryCode}")]
        public IActionResult UnblockCountry(string countryCode)
        {
            bool isRemoved = _blockService.UnblockCountry(countryCode);

            if (isRemoved)
                return Ok($"Country {countryCode} has been unblocked.");

            return NotFound($"Country {countryCode} was not found in the blocked list.");
        }

        //  Retrieve the list of blocked countries
        [HttpGet("blocked")]
        public IActionResult GetBlockedCountries([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest("Page and pageSize must be greater than zero.");

            var blockedCountries = _blockService.GetBlockedCountries(search, page, pageSize);

            return Ok(blockedCountries);
        }

        //  Temporarily block a country
        [HttpPost("temporal-block")]
        public IActionResult BlockCountryTemporarily([FromBody] TemporalBlockRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CountryCode))
                return BadRequest("Invalid country code.");

            if (request.DurationMinutes < 1 || request.DurationMinutes > 1440)
                return BadRequest("Duration must be between 1 and 1440 minutes.");

            bool isBlocked = _blockService.BlockCountryTemporarily(request.CountryCode, request.DurationMinutes);
            if (!isBlocked)
                return Conflict($"Country {request.CountryCode} is already temporarily blocked.");

            return Ok($"Country {request.CountryCode} has been temporarily blocked for {request.DurationMinutes} minutes.");
        }
    }
}
