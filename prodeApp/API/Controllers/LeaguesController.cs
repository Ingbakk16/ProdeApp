using Microsoft.AspNetCore.Mvc;
using ProdeApp.Application.Common;
using ProdeApp.Application.Dtos;
using ProdeApp.Application.Interfaces.Services;
using prodeApp.API.Contracts;

namespace prodeApp.API.Controllers;

[ApiController]
[Route("api/leagues")]
public sealed class LeaguesController : ControllerBase
{
    private readonly ILeagueService _leagueService;

    public LeaguesController(ILeagueService leagueService)
    {
        _leagueService = leagueService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _leagueService.GetAllAsync(cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value!.Select(ToBody))
            : ToError(result);
    }

    [HttpGet("{leagueId:int}")]
    public async Task<IActionResult> GetById(
        int leagueId,
        CancellationToken cancellationToken)
    {
        var result = await _leagueService.GetByIdAsync(leagueId, cancellationToken);

        return result.IsSuccess
            ? Ok(ToBody(result.Value!))
            : ToError(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateLeagueBody body,
        CancellationToken cancellationToken)
    {
        var request = new CreateLeagueRequest(
            body.Name,
            body.IsPrivate,
            body.Code,
            body.MaxMembers,
            body.FinishDate,
            body.TournamentId);

        var result = await _leagueService.CreateAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            return ToError(result);
        }

        var response = ToBody(result.Value!);

        return CreatedAtAction(
            nameof(GetById),
            new { leagueId = response.LeagueId },
            response);
    }

    private static LeagueBody ToBody(LeagueResponse response) => new(
        response.LeagueId,
        response.Name,
        response.IsPrivate,
        response.MaxMembers,
        response.FinishDate,
        response.OwnerId,
        response.TournamentId,
        response.MemberCount);

    private ObjectResult ToError<T>(Result<T> result)
    {
        var statusCode = result.ErrorCode switch
        {
            "unauthorized" => StatusCodes.Status401Unauthorized,
            "not_found" => StatusCodes.Status404NotFound,
            "conflict" => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest
        };

        return Problem(
            statusCode: statusCode,
            title: result.ErrorCode,
            detail: result.ErrorMessage);
    }
}
