using NinetyOneAssessment.Application.Models;

namespace NinetyOneAssessment.Application.Interfaces;

public interface IPersonRepository
{
    Task SaveAsync(IReadOnlyList<Person> people);
    Task<IReadOnlyList<Person>> GetByNameAsync(string firstName, string secondName);
    Task<IReadOnlyList<Person>> GetTopScorersAsync();
}