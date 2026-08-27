using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NinetyOneAssessment.Application;
using NinetyOneAssessment.Application.Exceptions;
using NinetyOneAssessment.Application.Interfaces;
using NinetyOneAssessment.Application.LoggingConfiguration;
using NinetyOneAssessment.Infrastructure;
using NinetyOneAssessment.Infrastructure.DbContexts;
using Serilog;


/*
 * Add in Db commands and calls
 * Add in minimal api endpoints
 * Update design docs
 * Include a drawio diagram detailing the architecture
 */

class Program
{
    private const int ExitSuccess = 0;
    private const int ExitFailure = 1;
    private const int ExitUsage = 2;

    private const string OutputDirectoryName = "Output";

    static async Task<int> Main(string[] args)
    {
        Log.Logger = SerilogConfiguration
            .CreateLoggerConfiguration(BuildConfiguration())
            .CreateLogger();

        try
        {
            return await RunAsync(args);
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    static async Task<int> RunAsync(string[] args)
    {
        if (args.Length == 1 && IsHelpRequest(args[0]))
        {
            WriteUsage(Console.Out);
            return ExitSuccess;
        }

        if (args.Length > 1)
        {
            Console.Error.WriteLine($"Error: expected at most one argument, found {args.Length}.");
            WriteUsage(Console.Error);
            return ExitUsage;
        }

        if (args.Length == 1 && string.IsNullOrWhiteSpace(args[0]))
        {
            Console.Error.WriteLine("Error: no input path supplied.");
            WriteUsage(Console.Error);
            return ExitUsage;
        }

        var requestedPath = args.Length == 1 ? args[0] : DefaultInputPath();
        string inputPath;
        try
        {
            inputPath = Path.GetFullPath(requestedPath);
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
        {
            Console.Error.WriteLine($"Error: '{requestedPath}' is not a usable file path. {ex.Message}");
            return ExitFailure;
        }

        if (Directory.Exists(inputPath))
        {
            Console.Error.WriteLine($"Error: '{inputPath}' is a directory, not a CSV file.");
            return ExitFailure;
        }

        if (!File.Exists(inputPath))
        {
            Console.Error.WriteLine($"Error: input file not found: '{inputPath}'");
            return ExitFailure;
        }

        var outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), OutputDirectoryName);

        try
        {
            using var host = CreateHostBuilder().Build();
            using var scope = host.Services.CreateScope();

            Log.Information("Application starting up. Reading {InputPath}", inputPath);
            
            var context = scope.ServiceProvider.GetRequiredService<ScoresDbContext>();
            await context.Database.MigrateAsync();

            var fileProcessingService = scope.ServiceProvider.GetRequiredService<IFileProcessingService>();
            var consoleWriterService = scope.ServiceProvider.GetRequiredService<IConsoleWriterService>();
            var personRepository = scope.ServiceProvider.GetRequiredService<IPersonRepository>();

            var output = await fileProcessingService.ProcessAsync(inputPath);
            consoleWriterService.Write(output.TopScorers, output.Failures);
            await fileProcessingService.SaveFileContentAsync(outputDirectory, output);
            
            //Save to db
            await personRepository.SaveAsync(output.People);

            Log.Information("Wrote {Count} top scorer(s) to {OutputDirectory}",
                output.TopScorers.Count, outputDirectory);

            return ExitSuccess;
        }
        catch (Exception ex) when (ex is FileNotFoundException or DirectoryNotFoundException)
        {
            Console.Error.WriteLine($"Error: input file not found: '{inputPath}'");
            Log.Error(ex, "Input file disappeared between validation and read: {InputPath}", inputPath);
            return ExitFailure;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Console.Error.WriteLine($"Error: could not read '{inputPath}'. {ex.Message}");
            Log.Error(ex, "I/O failure while processing {InputPath}", inputPath);
            return ExitFailure;
        }
        catch (CsvParseException ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            Log.Error(ex, "CSV parse failure in {InputPath}", inputPath);
            return ExitFailure;
        }
        catch (InvalidOperationException ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            Log.Error(ex, "CSV mapping failure in {InputPath}", inputPath);
            return ExitFailure;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: unexpected failure while processing '{inputPath}'. {ex.Message}");
            Log.Error(ex, "Unexpected failure while processing {InputPath}", inputPath);
            return ExitFailure;
        }
    }

    static string DefaultInputPath() =>
        Path.Combine(AppContext.BaseDirectory, "Assets", "TestData.csv");

    static bool IsHelpRequest(string argument) =>
        argument is "-h" or "--help" or "/?";

    static void WriteUsage(TextWriter writer)
    {
        writer.WriteLine("Usage: NinetyOneAssessment.Core [path-to-csv]");
        writer.WriteLine();
        writer.WriteLine("  path-to-csv  CSV file to read. Quote it if it contains spaces.");
        writer.WriteLine($"               Defaults to {DefaultInputPath()}");
        writer.WriteLine();
        writer.WriteLine($"Top scorers are written to STDOUT and to .\\{OutputDirectoryName}\\Top_Scorers.txt.");
        writer.WriteLine("Skipped records are reported on STDERR.");
        writer.WriteLine();
        writer.WriteLine("Exit codes: 0 success, 1 processing failure, 2 usage error.");
    }

    static IConfiguration BuildConfiguration() =>
        new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

    static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .UseContentRoot(AppContext.BaseDirectory)
            .UseSerilog()
            .ConfigureServices((context, services) =>
            {
                services.AddInfrastructureServices(context.Configuration);
                services.AddApplicationServices(context.Configuration);
            });
}
