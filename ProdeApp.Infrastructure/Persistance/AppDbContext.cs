using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProdeApp.Domain.Entities;

namespace ProdeApp.Infrastructure.Persistance
{
    

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<League> Leagues => Set<League>();
        public DbSet<LeagueMembership> LeagueMemberships => Set<LeagueMembership>();
        public DbSet<Pick> Picks => Set<Pick>();
        public DbSet<Match> Matches => Set<Match>();
        public DbSet<Tournament> Tournaments => Set<Tournament>();
        public DbSet<Team> Teams => Set<Team>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite key for LeagueMembership
            modelBuilder.Entity<LeagueMembership>()
                .HasKey(lm => new { lm.UserId, lm.LeagueId });

            // Composite unique key for Pick (one pick per user per match per league)
            modelBuilder.Entity<Pick>()
                .HasIndex(p => new { p.UserId, p.MatchId, p.LeagueId })
                .IsUnique();

            // Relationships for Match
            modelBuilder.Entity<Match>()
                .HasOne(m => m.HomeTeam)
                .WithMany(t => t.HomeMatches)
                .HasForeignKey(m => m.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayTeam)
                .WithMany(t => t.AwayMatches)
                .HasForeignKey(m => m.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
