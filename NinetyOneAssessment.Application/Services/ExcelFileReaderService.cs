using NinetyOneAssessment.Application.Interfaces;

namespace NinetyOneAssessment.Application.Services;

public class ExcelFileReaderService : IFileReader
{
    public List<string> ReadFile(string filePath)
    {
        throw new NotImplementedException();
    }

    public bool CanHandle(string fileExtension)
    {
        return fileExtension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase) 
               || fileExtension.Equals(".xls", StringComparison.OrdinalIgnoreCase) 
               || fileExtension.Equals(".xlsm", StringComparison.OrdinalIgnoreCase);
    }
}