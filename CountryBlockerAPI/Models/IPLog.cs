namespace CountryBlockerAPI.Models
{
    public class IPLog
    {
        public string IPAddress { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public DateTime AttemptTime { get; set; } = DateTime.UtcNow;
        public bool BlockedStatus { get; set; }
        public string UserAgent { get; set; } = string.Empty;
    }
}
