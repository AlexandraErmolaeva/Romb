using Romb.Application.Helpers;
using Romb.Application.Repositories;
using Romb.Application.Services;

namespace Romb.Application.Extensions
{
    public static class DependencyInjectionExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IPlannedEventService, PlannedEventService>();

            services.AddScoped<IActualEventService, ActualEventService>();

            services.AddScoped<IPlannedEventRepository, PlannedEventRepository>();

            services.AddScoped<IActualEventRepository, ActualEventRepository>();

            services.AddScoped<IBudgetCalculator, BudgetCalculator>();

            return services;
        }
    }
}
