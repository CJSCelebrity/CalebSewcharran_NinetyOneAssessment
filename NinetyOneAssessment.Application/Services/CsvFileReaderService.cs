using NinetyOneAssessment.Application.Interfaces;

namespace NinetyOneAssessment.Application.Services;

public class CsvFileReaderService : IFileReader
{
    public List<string> ReadFile(string filePath)
    {
        throw new NotImplementedException();
    }

    public bool CanHandle(string fileExtension)
    {
        return fileExtension.Equals(".csv", StringComparison.OrdinalIgnoreCase);
    }
}