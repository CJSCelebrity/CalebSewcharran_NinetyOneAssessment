namespace NinetyOneAssessment.Application.Models;

public record MappingResult(
    IReadOnlyList<Person> People,
    IReadOnlyList<RowFailure> Failures);