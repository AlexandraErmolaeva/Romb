using Romb.Application.Dtos;
using StackExchange.Redis;
using System.Text.Json;

namespace Romb.Application.Services;

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
        var jsonData = JsonSerializer.Serialize(value); 

        await _redisDatabase.StringSetAsync(key, jsonData, expiry);
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var jsonData = await _redisDatabase.StringGetAsync(key); 

        if (jsonData.IsNullOrEmpty)
            return default;

        return JsonSerializer.Deserialize<T>(jsonData);
    }

    public async Task RemoveAsync(string key)
    {
        await _redisDatabase.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _redisDatabase.KeyExistsAsync(key);
    }
}
