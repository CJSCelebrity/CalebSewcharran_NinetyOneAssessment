using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NinetyOneAssessment.Application.Interfaces;
using NinetyOneAssessment.Infrastructure.DbContexts;
using NinetyOneAssessment.Infrastructure.Repositories;

namespace NinetyOneAssessment.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ScoresDbContext>(options =>
            options.UseSqlite(SqliteConnectionStringResolver.Resolve(
                configuration.GetConnectionString(SqliteConnectionStringResolver.ConnectionStringName))));
        services.AddScoped<IPersonRepository, PersonRepository>();
        
        return services;
    }
}