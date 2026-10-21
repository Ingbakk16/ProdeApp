using ProdeApp.Application.Common;
using ProdeApp.Application.Dtos;

namespace ProdeApp.Application.Interfaces.Services;

public interface ILeagueService
{
    Task<Result<LeagueResponse>> CreateAsync(
        CreateLeagueRequest request,
        CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<LeagueResponse>>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<Result<LeagueResponse>> GetByIdAsync(
        int leagueId,
        CancellationToken cancellationToken);
}
