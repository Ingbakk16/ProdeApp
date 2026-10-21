namespace ProdeApp.Application.Dtos;

public sealed record CreateLeagueRequest(
    string Name,
    bool IsPrivate,
    string? Code,
    int? MaxMembers,
    DateTime? FinishDate,
    int TournamentId);

public sealed record LeagueResponse(
    int LeagueId,
    string Name,
    bool IsPrivate,
    int? MaxMembers,
    DateTime? FinishDate,
    int OwnerId,
    int TournamentId,
    int MemberCount);
