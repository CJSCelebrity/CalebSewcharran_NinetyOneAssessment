using NinetyOneAssessment.Application.Models;

namespace NinetyOneAssessment.Application.Interfaces;

public interface IFileProcessingService
{
    Task<ProcessingResult> ProcessAsync(string filePath);
    Task SaveFileContentAsync(string filePath, ProcessingResult results);
}