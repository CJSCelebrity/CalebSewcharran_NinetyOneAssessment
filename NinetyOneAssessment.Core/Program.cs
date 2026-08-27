using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NinetyOneAssessment.Application;
using NinetyOneAssessment.Application.Interfaces;
using NinetyOneAssessment.Application.LoggingConfiguration;
using NinetyOneAssessment.Infrastructure;
using Serilog;


/*
 * Add in Db commands and calls
 * Add in minimal api endpoints
 * Update design docs
 * Include a drawio diagram detailing the architecture
 */

class Program
{
    static async Task<int> Main(string[] args)
    {
        Log.Information("Application starting up");

        var host = CreateHostBuilder(args).Build();
        var fileProcessingService = host.Services.GetRequiredService<IFileProcessingService>();
        var consoleWriterService = host.Services.GetRequiredService<IConsoleWriterService>();
        
        var output = await fileProcessingService.ProcessAsync(@"C:\\Users\\User\\projects\\Caleb_Sewcharran_Ninety_One_Assessment\\NinetyOneAssessment.Core\\Assets\\TestData.csv");
        await fileProcessingService.SaveFileContentAsync(@"C:\Users\User\projects\Caleb_Sewcharran_Ninety_One_Assessment\NinetyOneAssessment.Core\Output", output);
        consoleWriterService.Write(output.TopScorers, output.Failures);
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
                services.AddInfrastructureServices(context.Configuration);
                services.AddApplicationServices(context.Configuration);
            });
}