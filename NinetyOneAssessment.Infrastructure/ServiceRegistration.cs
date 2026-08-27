using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NinetyOneAssessment.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }
}