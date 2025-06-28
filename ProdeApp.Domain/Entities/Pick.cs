using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdeApp.Domain.Entities
{
    public class Pick
    {
        public int PickId { get; set; }
        public int PredictedHomeGoals { get; set; }
        public int PredictedAwayGoals { get; set; }
        public int PointsAwarded { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int LeagueId { get; set; }
        public League League { get; set; } = null!;

        public int MatchId { get; set; }
        public Match Match { get; set; } = null!;
    }

}
