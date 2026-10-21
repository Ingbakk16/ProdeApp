using Microsoft.EntityFrameworkCore;
using ProdeApp.Application.Interfaces.Repositories;
using ProdeApp.Domain.Entities;

namespace ProdeApp.Infrastructure.Persistance.Repositories;

public sealed class LeagueRepository : ILeagueRepository
{
    private readonly AppDbContext _dbContext;

    public LeagueRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(League league, CancellationToken cancellationToken)
    {
        await _dbContext.Leagues.AddAsync(league, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<League>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _dbContext.Leagues
            .AsNoTracking()
            .Include(league => league.Members)
            .OrderBy(league => league.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<League?> GetByIdAsync(
        int leagueId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Leagues
            .AsNoTracking()
            .Include(league => league.Members)
            .SingleOrDefaultAsync(league => league.LeagueId == leagueId, cancellationToken);
    }
}
