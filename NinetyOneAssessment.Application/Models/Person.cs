namespace NinetyOneAssessment.Application.Models;

public record Person(string FirstName, string SecondName, int Score)
{
    public string? FullName =>  $"{FirstName} {SecondName}";
}