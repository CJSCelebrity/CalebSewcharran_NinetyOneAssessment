using Microsoft.EntityFrameworkCore;
using NinetyOneAssessment.Application.Models;

namespace NinetyOneAssessment.Infrastructure.DbContexts;

public class ScoresDbContext(DbContextOptions<ScoresDbContext> options) : DbContext(options)
{
    public DbSet<PersonEntity> People => Set<PersonEntity>();
}