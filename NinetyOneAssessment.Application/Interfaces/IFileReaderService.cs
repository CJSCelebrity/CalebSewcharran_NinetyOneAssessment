namespace NinetyOneAssessment.Infrastructure.Interfaces;

public interface IFileReaderService
{
    Task<string> ReadFile(string filePath);
}