using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdeApp.Domain.Entities
{
    public class League
    {
        public int LeagueId { get; set; }
        public string Name { get; set; } = null!;
        public bool IsPrivate { get; set; }
        public string? Code { get; set; }
        public int? MaxMembers { get; set; }
        public DateTime? FinishDate { get; set; }

        public int OwnerId { get; set; }
        public User Owner { get; set; } = null!;

        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; } = null!;

        public ICollection<LeagueMembership> Members { get; set; } = new List<LeagueMembership>();
        public ICollection<Pick> Picks { get; set; } = new List<Pick>();
    }
}
