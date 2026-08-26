

/*
 * TODO: Move to DESIGN.md
 * The goal, is to take an input of a csv file. Then output the top scorers in the console.
 * It will need to include the mark as well associated with the top  scoreers.
 *
 * If there are two or more top scorers, then the program should show them in alphabetical order (LINQ)
 *  Ensure that the implementation parses the CSV file from string and not with libraries
 *
 * support multiple file types, txt, csv, xlsx 
 */

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NinetyOneAssessment.Application;
using NinetyOneAssessment.Application.LoggingConfiguration;
using Serilog;

class Program
{
    static async Task Main(string[] args)
    {
        var configuration = BuildConfiguration(args);
        
        ConfigureLogging(configuration);

        try
        {
            Log.Information("Application starting up");

            var host = CreateHostBuilder(args, configuration).Build();

            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    private static IConfiguration BuildConfiguration(string[] args)
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();
    }

    private static void ConfigureLogging(IConfiguration configuration)
    {
        Log.Logger = SerilogConfiguration
            .CreateLoggerConfiguration(configuration)
            .CreateLogger();
    }
    
    static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((context, services) =>
            {
                services.AddApplicationServices(configuration);
            });
}