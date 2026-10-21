using ProdeApp.Application.Common;
using ProdeApp.Application.Dtos;
using ProdeApp.Application.Interfaces;
using ProdeApp.Application.Interfaces.Repositories;
using ProdeApp.Application.Interfaces.Services;
using ProdeApp.Domain.Entities;

namespace ProdeApp.Application.Services;

public sealed class LeagueService : ILeagueService
{
    private readonly ICurrentUser _currentUser;
    private readonly ILeagueRepository _leagueRepository;
    private readonly ITournamentRepository _tournamentRepository;

    public LeagueService(
        ICurrentUser currentUser,
        ILeagueRepository leagueRepository,
        ITournamentRepository tournamentRepository)
    {
        _currentUser = currentUser;
        _leagueRepository = leagueRepository;
        _tournamentRepository = tournamentRepository;
    }

    public async Task<Result<LeagueResponse>> CreateAsync(
        CreateLeagueRequest request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.UserId.HasValue)
        {
            return Result<LeagueResponse>.Failure("unauthorized", "An authenticated user is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Trim().Length > 100)
        {
            return Result<LeagueResponse>.Failure(
                "validation",
                "A league name is required and must be 100 characters or fewer.");
        }

        if (request.TournamentId <= 0)
        {
            return Result<LeagueResponse>.Failure(
                "validation",
                "A valid tournament is required.");
        }

        if (request.IsPrivate &&
            (string.IsNullOrWhiteSpace(request.Code) || request.Code.Trim().Length > 50))
        {
            return Result<LeagueResponse>.Failure(
                "validation",
                "Private leagues require a join code of 50 characters or fewer.");
        }

        if (request.MaxMembers is <= 0)
        {
            return Result<LeagueResponse>.Failure(
                "validation",
                "Maximum members must be greater than zero.");
        }

        var tournament = await _tournamentRepository.GetByIdAsync(
            request.TournamentId,
            cancellationToken);

        if (tournament is null)
        {
            return Result<LeagueResponse>.Failure("not_found", "The tournament was not found.");
        }

        var league = new League
        {
            Name = request.Name.Trim(),
            IsPrivate = request.IsPrivate,
            Code = request.IsPrivate ? request.Code!.Trim() : null,
            MaxMembers = request.MaxMembers,
            FinishDate = request.FinishDate,
            OwnerId = _currentUser.UserId.Value,
            TournamentId = tournament.TournamentId
        };

        await _leagueRepository.AddAsync(league, cancellationToken);

        return Result<LeagueResponse>.Success(ToResponse(league));
    }

    public async Task<Result<IReadOnlyList<LeagueResponse>>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var leagues = await _leagueRepository.GetAllAsync(cancellationToken);
        var response = leagues.Select(ToResponse).ToList();

        return Result<IReadOnlyList<LeagueResponse>>.Success(response);
    }

    public async Task<Result<LeagueResponse>> GetByIdAsync(
        int leagueId,
        CancellationToken cancellationToken)
    {
        var league = await _leagueRepository.GetByIdAsync(leagueId, cancellationToken);

        return league is null
            ? Result<LeagueResponse>.Failure("not_found", "The league was not found.")
            : Result<LeagueResponse>.Success(ToResponse(league));
    }

    private static LeagueResponse ToResponse(League league) => new(
        league.LeagueId,
        league.Name,
        league.IsPrivate,
        league.MaxMembers,
        league.FinishDate,
        league.OwnerId,
        league.TournamentId,
        league.Members.Count);
}
