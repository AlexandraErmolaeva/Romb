
namespace Romb.Application.Services
{
    public interface IRedisService
    {
        Task<bool> ExistsAsync(string key);
        Task<T> GetAsync<T>(string key);
        Task RemoveAsync(string key);
        Task SetAsync<T>(string key, IEnumerable<T> value, TimeSpan? expiry = null);
    }
}