using NinetyOneAssessment.Application.Interfaces;
using NinetyOneAssessment.Application.Models;

namespace NinetyOneAssessment.Application.Services;

public class FileProcessingService(IFileReaderFactory factory) : IFileProcessingService
{
    public IReadOnlyList<Person> ProcessFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            filePath = @"C:\\Users\\User\\projects\\Caleb_Sewcharran_Ninety_One_Assessment\\NinetyOneAssessment.Core\\Assets\\TestData.csv";
        
        var reader = factory.CreateFileReader(filePath);
        return reader.ReadFile(filePath);
    }

    public void PrintFileContentToConsole(IReadOnlyList<Person> results)
    {
        foreach (var item in results)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(item.Fullname);
        }
        
        Console.WriteLine($"Score: {results.FirstOrDefault()?.Score}");
        Console.ResetColor();
    }
    
    public async Task SaveFileContentAsync(string filePath, IReadOnlyList<Person> results)
    {
        await using var outputFile = new StreamWriter(Path.Combine(filePath, "Top_Scorers.txt"));
        foreach (var item in results)
        {
            await outputFile.WriteAsync($"{item.Fullname}\n");
        }
        await outputFile.WriteAsync("Score: " + results.FirstOrDefault()?.Score);
    }
}