using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NinetyOneAssessment.Infrastructure.DbContexts;

public class ScoresDbContextFactory : IDesignTimeDbContextFactory<ScoresDbContext>
{
    public ScoresDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ScoresDbContext>()
            .UseSqlite(SqliteConnectionStringResolver.Resolve(
                SqliteConnectionStringResolver.DefaultConnectionString))
            .Options;
        
        return new ScoresDbContext(options);
    }
}