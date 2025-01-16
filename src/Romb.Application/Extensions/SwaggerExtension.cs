using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Romb.Application.Extensions;
public static class SwaggerExtension
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services, string applicationName)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();

            // Добавляем XML-документацию, если доступна
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);

            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = applicationName,
                Version = "v1",
                Description = $"{applicationName} Docs"
            });
        });

        return services;
    }
}
