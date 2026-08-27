using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NinetyOneAssessment.Infrastructure.Interfaces;
using NinetyOneAssessment.Infrastructure.Services;

namespace NinetyOneAssessment.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IFileReaderService, FileReaderService>();
        return services;
    }
}