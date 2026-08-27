using NinetyOneAssessment.Application.Models;

namespace NinetyOneAssessment.Application.Interfaces;

public interface ICsvParserService
{
    IReadOnlyList<CsvRow> Parse(string content);
}