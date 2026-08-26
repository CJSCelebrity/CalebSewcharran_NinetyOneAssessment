using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NinetyOneAssessment.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }
}