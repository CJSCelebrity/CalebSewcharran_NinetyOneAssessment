using Microsoft.EntityFrameworkCore;
using NinetyOneAssessment.Application.Interfaces;
using NinetyOneAssessment.Application.Models;
using NinetyOneAssessment.Infrastructure.DbContexts;
using NinetyOneAssessment.Infrastructure.Mappers;

namespace NinetyOneAssessment.Infrastructure.Repositories;

public class PersonRepository(ScoresDbContext context) : IPersonRepository
{
    public async Task SaveAsync(IReadOnlyList<Person> people)
    {
        await context.People.ExecuteDeleteAsync();
        await context.People.AddRangeAsync(people.Select(p => p.ToEntity()));
        await context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Person>> GetByNameAsync(string firstName, string secondName)
    {
        var entities = await context.People
            .Where(p => p.FirstName == firstName && p.SecondName == secondName)
            .ToListAsync();

        return entities.Select(e => e.ToDomain()).ToArray();
    }

    public async Task<IReadOnlyList<Person>> GetTopScorersAsync()
    {
        if (!await context.People.AnyAsync())
            return [];

        var maxScore = await context.People.MaxAsync(p => p.Score);

        var entities = await context.People
            .Where(p => p.Score == maxScore)
            .ToListAsync();

        return entities
            .Select(e => e.ToDomain())
            .OrderBy(p => p.FullName, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}