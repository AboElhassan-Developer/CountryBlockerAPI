using CountryBlockerAPI.Models;
using System.Collections.Concurrent;

namespace CountryBlockerAPI.Services
{
    public class BlockService
    {
        private readonly ConcurrentDictionary<string, CountryBlock> _blockedCountries = new();
        private readonly List<IPLog> _blockedAttempts = new();

        //  Add a country to the permanent block list
        public bool BlockCountry(string countryCode)
        {
            if (_blockedCountries.ContainsKey(countryCode))
            {
                return false; //  Country is already blocked
            }

            _blockedCountries[countryCode] = new CountryBlock { CountryCode = countryCode, BlockedUntil = null };
            return true;
        }

        //  Add a country to the temporary block list
        public bool BlockCountryTemporarily(string countryCode, int durationMinutes)
        {
            if (_blockedCountries.ContainsKey(countryCode))
            {
                return false; // Country is already blocked, cannot duplicate
            }

            DateTime expirationTime = DateTime.UtcNow.AddMinutes(durationMinutes);
            _blockedCountries[countryCode] = new CountryBlock { CountryCode = countryCode, BlockedUntil = expirationTime };
            return true;
        }

        // Remove a country from the block list
        public bool UnblockCountry(string countryCode)
        {
            return _blockedCountries.TryRemove(countryCode, out _);
        }

        //  Check if a country is blocked
        public bool IsCountryBlocked(string countryCode)
        {
            if (_blockedCountries.TryGetValue(countryCode, out var block))
            {
                if (block.BlockedUntil == null || block.BlockedUntil > DateTime.UtcNow)
                    return true; // Country is still blocked

                //  Block has expired, remove the country automatically
                _blockedCountries.TryRemove(countryCode, out _);
            }
            return false;
        }

        //  Remove countries whose block has expired
        public void RemoveExpiredBlocks()
        {
            foreach (var country in _blockedCountries.Keys)
            {
                if (_blockedCountries.TryGetValue(country, out var block) && block.BlockedUntil <= DateTime.UtcNow)
                {
                    _blockedCountries.TryRemove(country, out _);
                }
            }
        }

        //  Log blocked access attempts
        public void LogBlockedAttempt(string ipAddress, string countryCode, bool isBlocked, string userAgent)
        {
            _blockedAttempts.Add(new IPLog
            {
                IPAddress = ipAddress,
                CountryCode = countryCode,
                AttemptTime = DateTime.UtcNow,
                BlockedStatus = isBlocked,
                UserAgent = userAgent
            });
        }

        //  Retrieve all blocked countries
        public IEnumerable<CountryBlock> GetBlockedCountries(string? search, int page, int pageSize)
        {
            var blockedList = _blockedCountries.Values.AsQueryable();

            //  Search by country code
            if (!string.IsNullOrWhiteSpace(search))
            {
                blockedList = blockedList.Where(c => c.CountryCode.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            //  Apply pagination
            return blockedList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        // Retrieve all blocked access attempts with pagination
        public IEnumerable<IPLog> GetBlockedAttempts(int page, int pageSize)
        {
            return _blockedAttempts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
    }
}
