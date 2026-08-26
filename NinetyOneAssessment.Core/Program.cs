using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NinetyOneAssessment.Application;
using NinetyOneAssessment.Application.Interfaces;
using NinetyOneAssessment.Application.LoggingConfiguration;
using Serilog;

class Program
{
    static async Task<int> Main(string[] args)
    {
        Log.Information("Application starting up");

        var host = CreateHostBuilder(args).Build();
        var fileProcessingService = host.Services.GetRequiredService<IFileProcessingService>();
        var output = fileProcessingService.ProcessFile(args.FirstOrDefault());
        fileProcessingService.PrintFileContentToConsole(output);
        await fileProcessingService.SaveFileContentAsync(@"C:\Users\User\projects\Caleb_Sewcharran_Ninety_One_Assessment\NinetyOneAssessment.Core\Output", output);
        //TODO: Update TaskFromResult to return fileProcessing result of 0 or 1
        return await Task.FromResult(0);
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