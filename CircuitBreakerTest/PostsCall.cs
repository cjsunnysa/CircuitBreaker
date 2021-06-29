using CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreakerTest
{
    public class PostsCall : IHttpCall
    {
        private readonly HttpClient _client;

        public PostsCall()
        {
            _client = new HttpClient();

            _client.Timeout = new TimeSpan(0, 0, 3);
        }
        public async Task<string> ExecuteAsync()
        {
            var request = new HttpRequestMessage(new HttpMethod("GET"), "https://jsonplaceholder.typicode.com/posts/1");

            var response = await _client.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }
    }
}
