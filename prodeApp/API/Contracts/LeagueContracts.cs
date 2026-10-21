using System.ComponentModel.DataAnnotations;

namespace prodeApp.API.Contracts;

public sealed class CreateLeagueBody
{
    [Required]
    [MaxLength(100)]
    public string Name { get; init; } = string.Empty;

    public bool IsPrivate { get; init; }

    [MaxLength(50)]
    public string? Code { get; init; }

    [Range(1, int.MaxValue)]
    public int? MaxMembers { get; init; }

    public DateTime? FinishDate { get; init; }

    [Range(1, int.MaxValue)]
    public int TournamentId { get; init; }
}

public sealed record LeagueBody(
    int LeagueId,
    string Name,
    bool IsPrivate,
    int? MaxMembers,
    DateTime? FinishDate,
    int OwnerId,
    int TournamentId,
    int MemberCount);
