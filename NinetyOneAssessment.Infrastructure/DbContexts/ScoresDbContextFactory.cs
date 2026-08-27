using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NinetyOneAssessment.Infrastructure.DbContexts;

public class ScoresDbContextFactory : IDesignTimeDbContextFactory<ScoresDbContext>
{
    public ScoresDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ScoresDbContext>()
            .UseSqlite("Data Source=scores.db")
            .Options;
        
        return new ScoresDbContext(options);
    }
}