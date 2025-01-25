
namespace Romb.Application.Services
{
    public interface IRedisService
    {
        Task<T> GetAsync<T>(string key, CancellationToken token = default);
        Task RemoveAsync(string key, CancellationToken token = default);
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken token = default);
    }
}