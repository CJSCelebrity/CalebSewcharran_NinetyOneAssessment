namespace NinetyOneAssessment.Application.Models;

public record Person
{
    public string? Firstname { get; init; }
    public string? Secondname { get; init; }
    public int Score { get; init; }
}