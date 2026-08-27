using NinetyOneAssessment.Application.Models;

namespace NinetyOneAssessment.Application.Interfaces;

public interface IConsoleWriterService
{
   void Write(IReadOnlyList<Person> topScorers, IReadOnlyList<RowFailure> failures);
}