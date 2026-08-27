using NinetyOneAssessment.Infrastructure.Interfaces;

namespace NinetyOneAssessment.Infrastructure.Services;

public class FileReaderService : IFileReaderService
{
    public async Task<string> ReadFile(string filePath)
    {
        return await File.ReadAllTextAsync(filePath);
    }
}