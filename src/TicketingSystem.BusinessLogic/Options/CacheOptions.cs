namespace TicketingSystem.BusinessLogic.Options
{
    public class CacheOptions(int responseCacheDuration = 60, double slidingExpiration = 1.0)
    {
        public int ResponseCacheDuration { get; set; } = responseCacheDuration;

        public double SlidingExpiration { get; set; } = slidingExpiration;
    }
}
