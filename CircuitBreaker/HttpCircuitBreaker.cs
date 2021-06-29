using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;

namespace CircuitBreaker
{
    public class HttpCircuitBreaker
    {
        private readonly Stopwatch _timeoutTime = new Stopwatch();
        private readonly CircuitBreakerConfiguration _configuration;
        private readonly IHttpCall _httpCall;
        private CircuitBreakerState _state = CircuitBreakerState.Closed;
        private int _failureCounter;
        private int _successCounter;

        public HttpCircuitBreaker(CircuitBreakerConfiguration configuration, IHttpCall httpCall)
        {
            _configuration = configuration;
            _httpCall = httpCall;
        }

        public async Task<string> ExecuteAsync()
        {
            switch (_state)
            {
                case CircuitBreakerState.Closed:
                    return await ExecuteClosedStateAsync();
                case CircuitBreakerState.HalfOpen:
                    return await ExecuteHalfOpenStateAsync();
                case CircuitBreakerState.Open:
                    return await ExecuteOpenStateAsync();
                default:
                    throw new InvalidOperationException("The circuit breaker's current state is invalid.");
            }
        }

        private async Task<string> ExecuteClosedStateAsync()
        {
            try
            {
                return await _httpCall.ExecuteAsync();
            }
            catch (Exception)
            {
                HandleCallFailure();
                throw;
            }
        }

        private async Task<string> ExecuteHalfOpenStateAsync()
        {
            try
            {
                var response = await _httpCall.ExecuteAsync();

                _successCounter++;

                if (_successCounter >= _configuration.SuccessThreshold)
                {
                    TransitionToState(CircuitBreakerState.Closed);
                }

                return response;
            }
            catch (Exception)
            {
                TransitionToState(CircuitBreakerState.Open);
                throw;
            }
        }
        
        private async Task<string> ExecuteOpenStateAsync()
        {
            if (_timeoutTime.ElapsedMilliseconds >= _configuration.TimoutInMilliseconds)
            {
                TransitionToState(CircuitBreakerState.HalfOpen);

                return await ExecuteAsync();
            }

            throw new CircuitBreakerOpenException($"Call aborted because the circuit breaker is open.");
        }

        private void HandleCallFailure()
        {
            _failureCounter++;

            if (_failureCounter >= _configuration.FailureThreshold)
            {
                TransitionToState(CircuitBreakerState.Open);
            }
        }

        public void TransitionToState(CircuitBreakerState newState)
        {
            switch (newState)
            {
                case CircuitBreakerState.Closed:
                    _failureCounter = 0;
                    break;
                case CircuitBreakerState.HalfOpen:
                    _timeoutTime.Stop();
                    _successCounter = 0;
                    break;
                case CircuitBreakerState.Open:
                    _timeoutTime.Restart();
                    break;
            }

            _state = newState;
        }

    }
}
