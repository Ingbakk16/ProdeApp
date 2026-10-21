using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProdeApp.Application.Interfaces.Repositories;
using ProdeApp.Infrastructure.Persistance;
using ProdeApp.Infrastructure.Persistance.Repositories;

namespace ProdeApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ILeagueRepository, LeagueRepository>();
        services.AddScoped<ITournamentRepository, TournamentRepository>();

        return services;
    }
}
