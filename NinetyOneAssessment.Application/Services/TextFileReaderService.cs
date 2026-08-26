using NinetyOneAssessment.Application.Interfaces;
using NinetyOneAssessment.Application.Models;

namespace NinetyOneAssessment.Application.Services;

public class TextFileReaderService : IFileReader
{
    public IReadOnlyList<Person> ReadFile(string filePath)
    {
        return new List<Person>();
    }

    public bool CanHandle(string fileExtension)
    {
        return fileExtension.Equals(".txt", StringComparison.OrdinalIgnoreCase);
    }
}