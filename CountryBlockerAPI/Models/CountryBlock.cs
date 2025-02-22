namespace CountryBlockerAPI.Models
{
    public class CountryBlock
    {
        public string CountryCode { get; set; } = string.Empty;
        public DateTime? BlockedUntil { get; set; } 
        public bool IsTemporarilyBlocked => BlockedUntil.HasValue;
    }
}
