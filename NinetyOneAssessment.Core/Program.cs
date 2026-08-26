using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NinetyOneAssessment.Application;
using NinetyOneAssessment.Application.Interfaces;
using NinetyOneAssessment.Application.LoggingConfiguration;
using Serilog;

class Program
{
    static Task<int> Main(string[] args)
    {
        Log.Information("Application starting up");

        var host = CreateHostBuilder(args).Build();
        var fileProcessingService = host.Services.GetRequiredService<IFileProcessingService>();
        var output = fileProcessingService.ProcessFile(args.FirstOrDefault());
        fileProcessingService.PrintFileContentToConsole(output);
        //TODO: Update TaskFromResult to return fileProcessing result of 0 or 1
        return Task.FromResult(0);
    }
    
    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog((context, loggerConfiguration) =>
            {
                Log.Logger = SerilogConfiguration
                    .CreateLoggerConfiguration(context.Configuration)
                    .CreateLogger();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddApplicationServices(context.Configuration);
            });
}