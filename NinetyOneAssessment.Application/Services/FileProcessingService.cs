using NinetyOneAssessment.Application.Interfaces;

namespace NinetyOneAssessment.Application.Services;

public class FileProcessingService(IFileReaderFactory factory) : IFileProcessingService
{
    public List<string> ProcessFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            filePath = @"C:\\Users\\User\\projects\\Caleb_Sewcharran_Ninety_One_Assessment\\NinetyOneAssessment.Core\\Assets\\TestData.csv";
        
        var reader = factory.CreateFileReader(filePath);
        return reader.ReadFile(filePath);
    }

    public void PrintFileContentToConsole(List<string> results)
    {
        foreach (var item in results)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(item);
        }
        Console.ResetColor();
    }
}