namespace NinetyOneAssessment.Application.Models;

public record CsvRow(
    int RecordNumber,
    IReadOnlyList<string> Fields);