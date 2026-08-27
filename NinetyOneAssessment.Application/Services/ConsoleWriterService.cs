using NinetyOneAssessment.Application.Interfaces;
using NinetyOneAssessment.Application.Models;

namespace NinetyOneAssessment.Application.Services;

public class ConsoleWriterService : IConsoleWriterService
{
    public void Write(IReadOnlyList<Person> topScorers, IReadOnlyList<RowFailure> failures)
    {
        foreach (var failure in failures)
            Console.Error.WriteLine(
                $"Skipped record {failure.RecordNumber}: {failure.Reason}");

        if (topScorers.Count == 0)
        {
            Console.WriteLine("No scores found.");
            return;
        }

        foreach (var person in topScorers)
            Console.WriteLine(person.FullName);

        Console.WriteLine($"Score: {topScorers.FirstOrDefault()?.Score}");
    }
}