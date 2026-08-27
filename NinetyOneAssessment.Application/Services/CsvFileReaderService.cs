using NinetyOneAssessment.Application.Interfaces;
using NinetyOneAssessment.Application.Models;

namespace NinetyOneAssessment.Application.Services;

public class CsvFileReaderService : IFileReader
{
    public IReadOnlyList<Person> ReadFile(string filePath)
    {
        var personList = new List<Person>();
        var lines = File.ReadAllLines(filePath);

        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) 
                continue;

            var values = line.Split(',');
            
            var person = new Person(
                FirstName: values[0].Trim(),
                SecondName: values[1].Trim(),
                Score: int.Parse(values[2].Trim()));
            
            personList.Add(person);
        }
        
        //Check if there are more than two top scorers
        var maxScore =  personList.Max(p => p.Score);
        var personsWithIdenticalScore = personList
            .Where(p => p.Score == maxScore)
            .OrderBy(person => person.FullName)
            .ToList();

        return personsWithIdenticalScore;
    }

    public bool CanHandle(string fileExtension)
    {
        return fileExtension.Equals(".csv", StringComparison.OrdinalIgnoreCase);
    }
}