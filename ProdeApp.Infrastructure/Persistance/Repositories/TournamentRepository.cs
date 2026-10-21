using Microsoft.EntityFrameworkCore;
using ProdeApp.Application.Interfaces.Repositories;
using ProdeApp.Domain.Entities;

namespace ProdeApp.Infrastructure.Persistance.Repositories;

public sealed class TournamentRepository : ITournamentRepository
{
    private readonly AppDbContext _dbContext;

    public TournamentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Tournament?> GetByIdAsync(
        int tournamentId,
        CancellationToken cancellationToken)
    {
        return _dbContext.Tournaments
            .AsNoTracking()
            .SingleOrDefaultAsync(
                tournament => tournament.TournamentId == tournamentId,
                cancellationToken);
    }
}
