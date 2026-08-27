namespace NinetyOneAssessment.Application.Models;

public record ProcessingResult(
    IReadOnlyList<Person> TopScorers,
    IReadOnlyList<RowFailure> Failures);