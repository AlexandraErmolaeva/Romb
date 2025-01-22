using Microsoft.Extensions.Configuration;
using Romb.Application.Services;
using StackExchange.Redis;

namespace Romb.Application.Extensions;

public static class RedisExtension
{
    public static IServiceCollection AddCustomRedis(this IServiceCollection services, IConfiguration configuration)
    {
        // Добавляем тяжеловесное соединение с редисом как синглтон.
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var redisConnection = configuration.GetConnectionString("Redis");

            return ConnectionMultiplexer.Connect(redisConnection);
        });
        // Добавляем редис-базу как трансиент, то есть на каждый инстанс отдельный экземпляр.
        services.AddTransient(sp =>
        {
            var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();

            return multiplexer.GetDatabase();
        });
        // Добавляем редис-сервис, который будет жить в контексте одного запроса.
        services.AddScoped<IRedisService, RedisService>();

        return services;
    }
}
