using Microsoft.AspNetCore.Mvc;
using NinetyOneAssessment.Api.Contracts;
using NinetyOneAssessment.Application.Interfaces;
using NinetyOneAssessment.Application.Models;

namespace NinetyOneAssessment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScoresController(IPersonRepository repository) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ScoreResponse>> Create(CreateScoreRequest request)
    {
        var person = new Person(request.FirstName, request.SecondName, request.Score);
        await repository.AddAsync(person);

        var response = MapToResponse(person);

        return CreatedAtAction(nameof(GetByName), new { firstName = person.FirstName, secondName = person.SecondName },
            response);
    }

    [HttpGet("TopScorers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TopScorerResponse>> GetTopScorers()
    {
        var topScorers = await repository.GetTopScorersAsync();

        return Ok(new TopScorerResponse(topScorers.Count == 0 ? 0 : topScorers[0].Score,
            topScorers.Select(MapToResponse).ToArray()));
    }

    [HttpGet("{firstName}/{secondName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ScoreResponse>>> GetByName(string firstName, string secondName)
    {
        var people = await repository.GetByNameAsync(firstName, secondName);

        if (people.Count == 0)
            return NotFound();

        return Ok(people.Select(MapToResponse).ToArray());
    }

    private static ScoreResponse MapToResponse(Person person) => new(person.FirstName, person.SecondName, person.Score);
}