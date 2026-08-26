using NinetyOneAssessment.Application.Interfaces;

namespace NinetyOneAssessment.Application.Services;

public class TextFileReaderService : IFileReader
{
    public List<string> ReadFile(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        return lines
            .Select(name => 
            {
                var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new 
                { 
                    FullName = name,
                    LastName = parts[^1],
                    GivenNames = string.Join(" ", parts[..^1])
                };
            })
            .OrderBy(x => x.LastName)
            .ThenBy(y => y.GivenNames)
            .Select(z => z.FullName)
            .ToList();
    }

    public bool CanHandle(string fileExtension)
    {
        return fileExtension.Equals(".txt", StringComparison.OrdinalIgnoreCase);
    }
}