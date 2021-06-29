using CircuitBreaker;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CircuitBreakerTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new CircuitBreakerConfiguration
            {
                FailureThreshold = 3,
                SuccessThreshold = 3,
                TimoutInMilliseconds = 5000
            };

            var call = new PostsCall();

            var circuitBreaker = new HttpCircuitBreaker(configuration, call);

            for (var i = 0; i < 30; i++)
            {
                await ExecuteCallAsync(circuitBreaker);

                Thread.Sleep(1000);
            }
        }

        private static async Task ExecuteCallAsync(HttpCircuitBreaker circuitBreaker)
        {
            try
            {
                var response = await circuitBreaker.ExecuteAsync();

                Console.WriteLine(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
