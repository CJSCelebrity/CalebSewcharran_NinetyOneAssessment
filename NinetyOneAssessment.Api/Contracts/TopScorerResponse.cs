namespace NinetyOneAssessment.Api.Contracts;

public record TopScorerResponse(int Score, IReadOnlyList<ScoreResponse> People);