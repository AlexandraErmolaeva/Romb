using Microsoft.EntityFrameworkCore;
using Romb.Application;
using Romb.Application.Helpers;
using Romb.Application.Extensions;
using Romb.Application.Mappers;
using Romb.Application.Middleware;
using Romb.Application.Services;
using Serilog;
using System.Reflection;
using Romb.Application.Repositories;

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

LogInformation($"- Adding {nameof(IPlannedEventService)}");
services.AddScoped<IPlannedEventService, PlannedEventService>();

LogInformation($"- Adding {nameof(IActualEventService)}");
services.AddScoped<IActualEventService, ActualEventService>();

LogInformation($"- Adding {nameof(IPlannedEventRepository)}");
services.AddScoped<IPlannedEventRepository, PlannedEventRepository>();

LogInformation($"- Adding {nameof(IActualEventRepository)}");
services.AddScoped<IActualEventRepository, ActualEventRepository>();

LogInformation($"- Adding {nameof(IBudgetCalculator)}");
services.AddScoped<IBudgetCalculator, BudgetCalculator>();

LogInformation("- Adding AutoMapper...");
services.AddAutoMapper(typeof(PlanedEventMappingProfile));

LogInformation("- Adding Controllers...");
services.AddControllers();

LogInformation("- Adding DbContext...");
services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

LogInformation("- Bilding App...");
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    LogInformation("- Configuruing Swagger...");
    app.UseSwagger();
    app.UseSwaggerUI(); 
}

LogInformation("- Using Https Redirection...");
app.UseHttpsRedirection();

LogInformation($"- Using {nameof(ErrorHandlingMiddleware)}...");
app.UseMiddleware<ErrorHandlingMiddleware>();

LogInformation("- Using Routing...");
app.UseRouting();

LogInformation("- Using Https Endpoints...");
app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
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