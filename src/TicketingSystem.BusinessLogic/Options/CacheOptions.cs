namespace TicketingSystem.BusinessLogic.Options
{
    public class CacheOptions
    {
        public static readonly string ConfigurationKey = "Cache";

        public int ResponseCacheDuration { get; set; } = 60;

        public double SlidingExpiration { get; set; } = 1.0;
    }
}
