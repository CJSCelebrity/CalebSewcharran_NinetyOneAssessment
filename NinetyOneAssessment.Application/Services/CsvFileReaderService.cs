using NinetyOneAssessment.Application.Interfaces;
using NinetyOneAssessment.Application.Models;

namespace NinetyOneAssessment.Application.Services;

/*
 * Once the data has been read, we can either modify them and place them in its own model and then display it to the console
 */


public class CsvFileReaderService : IFileReader
{
    public List<string> ReadFile(string filePath)
    {
        var personList = new List<Person>();
        var lines = File.ReadAllLines(filePath);

        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) 
                continue;

            var values = line.Split(',');
            personList.Add(new Person
            {
                Firstname =  values[0].Trim(),
                Secondname =  values[1].Trim(),
                Score = int.Parse(values[2].Trim())
            });
        }
        
       
        return new List<string>();
        // string[] read;
        // char[] seperators = [','];
        //
        // var streamReader = new StreamReader(filePath);
        // var data = streamReader.ReadToEnd();
        //
        // return new List<string>(data.Split(seperators));
    }

    public bool CanHandle(string fileExtension)
    {
        return fileExtension.Equals(".csv", StringComparison.OrdinalIgnoreCase);
    }
}