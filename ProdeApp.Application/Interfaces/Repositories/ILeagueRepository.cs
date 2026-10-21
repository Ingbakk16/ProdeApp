using ProdeApp.Domain.Entities;

namespace ProdeApp.Application.Interfaces.Repositories;

public interface ILeagueRepository
{
    Task AddAsync(League league, CancellationToken cancellationToken);
    Task<IReadOnlyList<League>> GetAllAsync(CancellationToken cancellationToken);
    Task<League?> GetByIdAsync(int leagueId, CancellationToken cancellationToken);
}
