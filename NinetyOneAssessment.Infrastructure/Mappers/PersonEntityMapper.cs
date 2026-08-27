using NinetyOneAssessment.Application.Models;

namespace NinetyOneAssessment.Infrastructure.Mappers;

public static class PersonEntityMapper
{
    public static PersonEntity ToEntity(this Person person) => new()
    {
        FirstName = person.FirstName,
        SecondName = person.SecondName,
        Score = person.Score
    };

    public static Person ToDomain(this PersonEntity entity) =>
        new(entity.FirstName, entity.SecondName, entity.Score);
}