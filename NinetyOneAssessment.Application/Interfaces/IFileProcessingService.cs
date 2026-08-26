namespace NinetyOneAssessment.Application.Interfaces;

public interface IFileProcessingService
{
    List<string> ProcessFile(string? filePath);
    void PrintFileContentToConsole(List<string> results);
}