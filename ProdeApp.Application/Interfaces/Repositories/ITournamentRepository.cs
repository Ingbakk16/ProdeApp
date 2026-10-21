using ProdeApp.Domain.Entities;

namespace ProdeApp.Application.Interfaces.Repositories;

public interface ITournamentRepository
{
    Task<Tournament?> GetByIdAsync(int tournamentId, CancellationToken cancellationToken);
}
