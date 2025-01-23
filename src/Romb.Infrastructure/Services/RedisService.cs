using Romb.Application.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace Romb.Infrastructure;

public class RedisService : IRedisService
{
    private readonly IDatabase _redisDatabase;
    private readonly ILogger<RedisService> _logger;

    private const string ServiceName = nameof(RedisService);

    public RedisService(IDatabase redisDatabase, ILogger<RedisService> logger)
    {
        _redisDatabase = redisDatabase;
        _logger = logger;
    }

    public async Task SetAsync<T>(string key, IEnumerable<T> value, TimeSpan? expiry = null)
    {
        _logger.LogInformation("[{ServiceName}]: Data is being written to Redis...", ServiceName);

        var jsonData = JsonSerializer.Serialize(value);

        await _redisDatabase.StringSetAsync(key, jsonData, expiry);

        _logger.LogInformation("[{ServiceName}]: The data has been successfully written to Redis with key: {Key}.", ServiceName, key);
    }

    public async Task<T> GetAsync<T>(string key)
    {
        _logger.LogInformation("[{ServiceName}]: Trying to get data in Redis with current key: {Key}...", ServiceName, key);

        var isKeyExist = await ExistsAsync(key);

        if (!isKeyExist)
        {
            _logger.LogWarning("[{ServiceName}]: Redis doesnt contain current key.", ServiceName);
            return default;
        }

        var jsonData = await _redisDatabase.StringGetAsync(key);

        if (jsonData.IsNullOrEmpty)
        {
            _logger.LogWarning("[{ServiceName}]: Data is empty or null.", ServiceName);
            return default;
        }

        _logger.LogInformation("[{ServiceName}]: The data has been successfully get from Redis with key: {Key}.", ServiceName, key);

        return JsonSerializer.Deserialize<T>(jsonData);
    }

    public async Task RemoveAsync(string key)
    {
        var isKeyExist = await ExistsAsync(key);

        if (!isKeyExist)
        {
            _logger.LogWarning("[{ServiceName}]: Redis doesnt contain current key.", ServiceName);
            return;
        }

        await _redisDatabase.KeyDeleteAsync(key);

        _logger.LogInformation("[{ServiceName}]: Data has been deleted in Redis with key: {Key}.", ServiceName, key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _redisDatabase.KeyExistsAsync(key);
    }
}
