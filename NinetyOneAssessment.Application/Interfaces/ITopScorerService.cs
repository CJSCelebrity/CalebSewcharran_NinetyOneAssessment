using NinetyOneAssessment.Application.Models;

namespace NinetyOneAssessment.Application.Interfaces;

public interface ITopScorerService
{
    IReadOnlyList<Person> GetTopScorers(IReadOnlyList<Person> people);
}