using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();


//using Microsoft.Extensions.Hosting;

//var host = new HostBuilder()
//    .ConfigureFunctionsWorkerDefaults()
//    .ConfigureAppConfiguration(config =>
//    {
//        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
//        config.AddEnvironmentVariables();
//    })
//    .ConfigureServices(services =>
//    {
//        // Optional: Dependency injection
//    })
//    .Build();

//host.Run();
