namespace CircuitBreaker
{
    public class CircuitBreakerConfiguration
    {
        public int FailureThreshold { get; set; }
        public int SuccessThreshold { get; set; }
        public long TimoutInMilliseconds { get; set; }
    }
}