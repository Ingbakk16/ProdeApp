using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdeApp.Domain.Entities
{
    public class Tournament
    {
        public int TournamentId { get; set; }
        public string TournamentName { get; set; } = null!;
        public string? SourceApi { get; set; }
        public bool IsActive { get; set; }

        public ICollection<League> Leagues { get; set; } = new List<League>();
        public ICollection<Match> Matches { get; set; } = new List<Match>();
    }
}
