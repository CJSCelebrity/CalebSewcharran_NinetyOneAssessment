using NinetyOneAssessment.Application.Interfaces;
using NinetyOneAssessment.Application.Mappers;
using NinetyOneAssessment.Application.Models;
using NinetyOneAssessment.Infrastructure.Interfaces;

namespace NinetyOneAssessment.Application.Services;

public class FileProcessingService(ITopScorerService topScorerService, IFileReaderService fileReaderService, ICsvParserService csvParser) : IFileProcessingService
{
    public async Task<ProcessingResult> ProcessAsync(string filePath)
    {
        var fileData = await fileReaderService.ReadFile(filePath);
        var rows = csvParser.Parse(fileData);
        var mapping = PersonMapper.Map(rows);
        var topScorers = topScorerService.GetTopScorers(mapping.People);
        
        return new ProcessingResult(topScorers, mapping.Failures);
    }

    public async Task SaveFileContentAsync(string filePath, ProcessingResult results)
    {
        await using var outputFile = new StreamWriter(Path.Combine(filePath, "Top_Scorers.txt"));
        foreach (var item in results.TopScorers)
        {
            await outputFile.WriteAsync($"{item.FullName}\n");
        }
        await outputFile.WriteAsync("Score: " + results.TopScorers.FirstOrDefault()?.Score);
    }
}