using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Romb.Application;
using Romb.Application.Extensions;
using Romb.Application.Mappers;
using Romb.Application.Middleware;
using Serilog;
using System.Reflection;

var assemblyName = Assembly.GetExecutingAssembly().GetName();

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Host.UseSerilog();

PrintApplicationInfo(assemblyName);

LogInformation("- Adding Redis...");
services.AddCustomRedis(configuration);

LogInformation("- Adding Swagger...");
services.AddCustomSwagger(assemblyName.Name);

LogInformation("- Adding Services...");
services.AddServices();

builder.Services.AddHealthChecks()
    .AddRedis(configuration.GetConnectionString("Redis"), name: "redis", failureStatus: HealthStatus.Unhealthy);

LogInformation($"- Adding AutoMapper: {nameof(PlannedEventMappingProfile)}...");
services.AddAutoMapper(typeof(PlannedEventMappingProfile));

LogInformation($"- Adding AutoMapper: {nameof(ActualEventMappingProfile)}...");
services.AddAutoMapper(typeof(ActualEventMappingProfile)); ;

LogInformation("- Adding Controllers...");
services.AddControllers();

LogInformation("- Adding DbContext...");
services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("NpgsqlDefaultConnection")));

LogInformation("- Adding Controllers With Views...");
services.AddControllersWithViews();

LogInformation("- Bilding App...");
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    LogInformation("- Configuruing Swagger...");
    app.UseSwagger();
    app.UseSwaggerUI(); 
}

app.UseHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true
});

LogInformation("- Using Https Redirection...");
app.UseHttpsRedirection();

LogInformation($"- Using {nameof(ErrorHandlingMiddleware)}...");
app.UseMiddleware<ErrorHandlingMiddleware>();

LogInformation("- Using Routing...");
app.UseRouting();

LogInformation("- Using Static Files...");
app.UseStaticFiles();

LogInformation("- Using Https Endpoints...");
app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();

    _ = endpoints.MapFallbackToFile("index.html");
});

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}

static void PrintApplicationInfo(AssemblyName assemblyName)
{
    var separator = new string('-', 30);
    
    var applicationName = assemblyName.Name;
    var applicationVersion = assemblyName.Version?.ToString() ?? "unknown";

    var machineName = Environment.MachineName;
    var operatingSystem = Environment.OSVersion.ToString();
    var dotNetVersion = Environment.Version.ToString();
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
    var currentDirectory = Environment.CurrentDirectory;
    var processorCount = Environment.ProcessorCount;
    var startTime = DateTime.UtcNow;

    LogHeader("Application Info", separator);
    LogInformation("Application Name    : {ApplicationName}", applicationName);
    LogInformation("Application Version : {ApplicationVersion}", applicationVersion);
    LogInformation("Machine Name        : {MachineName}", machineName);
    LogInformation("Operating System    : {OperatingSystem}", operatingSystem);
    LogInformation("Environment         : {Environment}", environment);
    LogInformation("DotNet Version      : {DotNetVersion}", dotNetVersion);
    LogInformation("Processor Count     : {ProcessorCount}", processorCount);
    LogInformation("Current Directory   : {CurrentDirectory}", currentDirectory);
    LogInformation("Start Time (UTC)    : {StartTime}", startTime);
}

static void LogHeader(string title, string separator)
{
    LogInformation(separator);
    LogInformation($"  {title}");
    LogInformation(separator);
}

static void LogInformation(string message, params object[] propertyValues)
{
    Log.Information(message, propertyValues);
}