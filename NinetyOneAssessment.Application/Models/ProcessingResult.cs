namespace NinetyOneAssessment.Application.Models;

public record ProcessingResult(
    IReadOnlyList<Person> People,
    IReadOnlyList<Person> TopScorers,
    IReadOnlyList<RowFailure> Failures);