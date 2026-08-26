using NinetyOneAssessment.Application.Models;

namespace NinetyOneAssessment.Application.Interfaces;

public interface IFileReader
{
    IReadOnlyList<Person> ReadFile(string filePath); 
    bool CanHandle(string fileExtension); 
}