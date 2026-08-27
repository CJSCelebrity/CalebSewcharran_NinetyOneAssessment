using System.Text;
using NinetyOneAssessment.Application.Exceptions;
using NinetyOneAssessment.Application.Interfaces;
using NinetyOneAssessment.Application.Models;

namespace NinetyOneAssessment.Application.Services;

public sealed class CsvParserService : ICsvParserService
{
    public IReadOnlyList<CsvRow> Parse(string content)
    {
        var rows = new List<CsvRow>();
        if (string.IsNullOrEmpty(content))
            return rows;

        var fields = new List<string>();
        var buffer = new StringBuilder();
        var inQuotes = false;
        var fieldWasQuoted = false;
        var recordHasContent = false;
        var recordNumber = 1;

        for (var i = 0; i < content.Length; i++)
        {
            var c = content[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    // A doubled quote inside a quoted field is a literal quote.
                    if (i + 1 < content.Length && content[i + 1] == '"')
                    {
                        buffer.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    // Commas and newlines are literal text while quoted.
                    buffer.Append(c);
                }

                continue;
            }

            switch (c)
            {
                case '"':
                    inQuotes = true;
                    fieldWasQuoted = true;
                    recordHasContent = true;
                    break;

                case ',':
                    fields.Add(TakeField(buffer, fieldWasQuoted));
                    fieldWasQuoted = false;
                    recordHasContent = true;
                    break;

                case '\r':
                    // Treat CRLF as a single record separator.
                    if (i + 1 < content.Length && content[i + 1] == '\n')
                        i++;
                    goto case '\n';

                case '\n':
                    fields.Add(TakeField(buffer, fieldWasQuoted));
                    rows.Add(new CsvRow(recordNumber++, fields.ToArray()));
                    fields.Clear();
                    fieldWasQuoted = false;
                    recordHasContent = false;
                    break;

                default:
                    buffer.Append(c);
                    recordHasContent = true;
                    break;
            }
        }

        if (inQuotes)
            throw new CsvParseException(
                $"Unterminated quoted field in record {recordNumber}.");

        // Flush the final record only when the file did not end on a separator,
        // otherwise a trailing newline yields a false empty row.
        if (recordHasContent || buffer.Length > 0 || fields.Count > 0)
        {
            fields.Add(TakeField(buffer, fieldWasQuoted));
            rows.Add(new CsvRow(recordNumber, fields.ToArray()));
        }

        return rows;
    }

    private static string TakeField(StringBuilder buffer, bool wasQuoted)
    {
        var value = buffer.ToString();
        buffer.Clear();
        // Only unquoted fields are trimmed — whitespace inside quotes is data.
        return wasQuoted ? value : value.Trim();
    }
}