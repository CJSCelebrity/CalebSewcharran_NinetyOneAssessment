using NinetyOneAssessment.Application.Interfaces;
using NinetyOneAssessment.Application.Services;

namespace NinetyOneAssessment.Application.Factories;

public class FileReaderFactory : IFileReaderFactory
{
    private readonly List<IFileReader> _readers = new()
    {
        new TextFileReaderService(),
        new CsvFileReaderService(),
        new ExcelFileReaderService()
    };
    
    public IFileReader CreateFileReader(string path)
    {
        var extension = Path.GetExtension(path);
        var reader = _readers.FirstOrDefault(x => x.CanHandle(extension));

        return reader ?? throw new NotSupportedException($"No reader available for file extension: {extension}");
    }

    public void RegisterReader(IFileReader reader)
    {
        _readers.Add(reader);
    }
}