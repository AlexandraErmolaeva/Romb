using Microsoft.Extensions.Configuration;
using Romb.Application.Services;
using StackExchange.Redis;

namespace Romb.Application.Extensions;

public static class RedisExtension
{
    public static IServiceCollection AddCustomRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var redisConnection = configuration.GetConnectionString("Redis");

            return ConnectionMultiplexer.Connect(redisConnection);
        });

        services.AddTransient(sp =>
        {
            var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();

            return multiplexer.GetDatabase();
        });

        services.AddScoped<IRedisService, RedisService>();

        return services;
    }
}
