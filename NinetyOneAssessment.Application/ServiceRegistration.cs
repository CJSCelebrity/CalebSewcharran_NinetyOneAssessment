using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NinetyOneAssessment.Application.Interfaces;
using NinetyOneAssessment.Application.Services;
using NinetyOneAssessment.Infrastructure.Interfaces;
using NinetyOneAssessment.Infrastructure.Services;

namespace NinetyOneAssessment.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IConsoleWriterService, ConsoleWriterService>();
        services.AddTransient<ITopScorerService, TopScorerService>();
        services.AddTransient<ICsvParserService, CsvParserService>();
        services.AddTransient<IFileReaderService, FileReaderService>();
        services.AddTransient<IFileProcessingService, FileProcessingService>();
        services.AddTransient<IFileReader, CsvFileReaderService>();
        return services;
    }
}