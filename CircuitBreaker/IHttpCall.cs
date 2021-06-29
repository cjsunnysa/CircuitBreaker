using System.Threading.Tasks;

namespace CircuitBreaker
{
    public interface IHttpCall
    {
        Task<string> ExecuteAsync();
    }
}