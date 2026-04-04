using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;

namespace TalentFlow.Infrastructure.Resilience
{
    public static class PollyPolicies
    {
        public static AsyncRetryPolicy RetryPolicy =>
            Policy.Handle<Exception>()
                  .WaitAndRetryAsync(
                      retryCount: 3,
                      sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // exponential backoff
                      onRetry: (exception, timespan, attempt, context) =>
                      {
                          Console.WriteLine($"Retry {attempt} after {timespan.TotalSeconds}s due to {exception.Message}");
                      });

        public static AsyncCircuitBreakerPolicy CircuitBreakerPolicy =>
            Policy.Handle<Exception>()
                  .CircuitBreakerAsync(
                      exceptionsAllowedBeforeBreaking: 2,
                      durationOfBreak: TimeSpan.FromSeconds(30),
                      onBreak: (exception, timespan) =>
                      {
                          Console.WriteLine($"Circuit broken for {timespan.TotalSeconds}s due to {exception.Message}");
                      },
                      onReset: () => Console.WriteLine("Circuit reset"),
                      onHalfOpen: () => Console.WriteLine("Circuit half-open, next call is a trial"));
    }
}
