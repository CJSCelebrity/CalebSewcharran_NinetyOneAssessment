using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NinetyOneAssessment.Application.Factories;
using NinetyOneAssessment.Application.Interfaces;
using NinetyOneAssessment.Application.Services;

namespace NinetyOneAssessment.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IFileReaderFactory, FileReaderFactory>();
        services.AddTransient<IFileProcessingService, FileProcessingService>();
        services.AddTransient<IFileReader, CsvFileReaderService>();
        services.AddTransient<IFileReader, TextFileReaderService>();
        return services;
    }
}