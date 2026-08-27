using NinetyOneAssessment.Application.Interfaces;
using NinetyOneAssessment.Application.Models;

namespace NinetyOneAssessment.Application.Services;

public class TopScorerService : ITopScorerService
{
    public IReadOnlyList<Person> GetTopScorers(IReadOnlyList<Person> people)
    {
        if (people.Count == 0)
            return [];

        var maxScore = people.Max(p => p.Score);

        return people
            .Where(p => p.Score == maxScore)
            .OrderBy(p => p.FullName, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}