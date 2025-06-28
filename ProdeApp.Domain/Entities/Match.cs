using ProdeApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdeApp.Domain.Entities
{
    public class Match
    {
        public int MatchId { get; set; }
        public int HomeTeamId { get; set; }
        public Team HomeTeam { get; set; } = null!;

        public int AwayTeamId { get; set; }
        public Team AwayTeam { get; set; } = null!;

        public DateTime MatchDate { get; set; }
        public int ResultHomeGoals { get; set; }
        public int ResultAwayGoals { get; set; }
        public MatchStatus MatchStatus { get; set; }

        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; } = null!;

        public ICollection<Pick> Picks { get; set; } = new List<Pick>();
    }
}
