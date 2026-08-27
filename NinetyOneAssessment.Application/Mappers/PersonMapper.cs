using NinetyOneAssessment.Application.Models;

namespace NinetyOneAssessment.Application.Mappers;

public static class PersonMapper
{
    const string FirstNameHeader = "First Name";
    const string SecondNameHeader = "Second Name";
    const string ScoreHeader = "Score";
    
    public static MappingResult Map(IReadOnlyList<CsvRow> rows)
    {
        var people = new List<Person>();
        var failures = new List<RowFailure>();

        if (rows.Count == 0)
            return new MappingResult(people, failures);

        var header = rows[0].Fields;
        var firstIndex = IndexOf(header, FirstNameHeader);
        var secondIndex = IndexOf(header, SecondNameHeader);
        var scoreIndex = IndexOf(header, ScoreHeader);

        if (firstIndex < 0 || secondIndex < 0 || scoreIndex < 0)
            throw new InvalidOperationException(
                $"Header must contain '{FirstNameHeader}', '{SecondNameHeader}' and '{ScoreHeader}'.");

        var widthNeeded = Math.Max(firstIndex, Math.Max(secondIndex, scoreIndex)) + 1;

        foreach (var row in rows.Skip(1))
        {
            if (row.Fields.Count < widthNeeded)
            {
                failures.Add(new RowFailure(row.RecordNumber,
                    $"Expected at least {widthNeeded} fields, found {row.Fields.Count}."));
                continue;
            }

            var firstName = row.Fields[firstIndex];
            var secondName = row.Fields[secondIndex];
            var rawScore = row.Fields[scoreIndex];

            if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(secondName))
            {
                failures.Add(new RowFailure(row.RecordNumber, "Name is empty."));
                continue;
            }

            if (!int.TryParse(rawScore, out var score))
            {
                failures.Add(new RowFailure(row.RecordNumber,
                    $"Score '{rawScore}' is not a whole number."));
                continue;
            }

            people.Add(new Person(firstName, secondName, score));
        }

        return new MappingResult(people, failures);
    }
    
    private static int IndexOf(IReadOnlyList<string> header, string name)
    {
        for (var i = 0; i < header.Count; i++)
            if (string.Equals(header[i], name, StringComparison.OrdinalIgnoreCase))
                return i;
        return -1;
    }
}