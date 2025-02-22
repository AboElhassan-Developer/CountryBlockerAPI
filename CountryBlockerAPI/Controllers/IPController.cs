using Microsoft.AspNetCore.Mvc;
using CountryBlockerAPI.Services;
using System.Net;

namespace CountryBlockerAPI.Controllers
{
    [Route("api/ip")]
    [ApiController]
    public class IPController : ControllerBase
    {
        private readonly GeoLocationService _geoLocationService;
        private readonly BlockService _blockService;

        public IPController(GeoLocationService geoLocationService, BlockService blockService)
        {
            _geoLocationService = geoLocationService;
            _blockService = blockService;
        }

        //  Retrieve country data using IP
        [HttpGet("lookup")]
        public async Task<IActionResult> LookupIP([FromQuery] string? ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            }

            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                return BadRequest("Could not determine your IP address.");
            }

            var locationResult = await _geoLocationService.GetCountryByIP(ipAddress);
            if (locationResult == null)
            {
                return NotFound("Could not determine the country for this IP.");
            }

            return Ok(new
            {
                IP = ipAddress,
                CountryCode = locationResult,
            });
        }

        // Check if the IP is blocked
        [HttpGet("check-block")]
        public async Task<IActionResult> CheckIPBlock([FromQuery] string? ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            }

            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                return BadRequest("Could not determine your IP address.");
            }

            var countryCode = await _geoLocationService.GetCountryByIP(ipAddress);
            if (countryCode == null)
            {
                return NotFound("Could not determine the country for this IP.");
            }

            bool isBlocked = _blockService.IsCountryBlocked(countryCode);
            string userAgent = Request.Headers["User-Agent"].ToString();

            // Log the blocked attempt
            _blockService.LogBlockedAttempt(ipAddress, countryCode, isBlocked, userAgent);

            return Ok(new
            {
                IP = ipAddress,
                CountryCode = countryCode,
                Blocked = isBlocked
            });
        }
    }
}
