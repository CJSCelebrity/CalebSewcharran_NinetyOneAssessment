using NinetyOneAssessment.Application.Models;

namespace NinetyOneAssessment.Application.Interfaces;

public interface IFileProcessingService
{
    IReadOnlyList<Person> ProcessFile(string? filePath);
    void PrintFileContentToConsole(IReadOnlyList<Person> results);
    Task SaveFileContentAsync(string filePath, IReadOnlyList<Person> results);
}