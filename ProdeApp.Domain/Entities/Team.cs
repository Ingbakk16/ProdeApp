using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdeApp.Domain.Entities
{
    public class Team
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = null!;
        public string? TeamShortcode { get; set; }
        public string? LogoUrl { get; set; }
        public string? Country { get; set; }

        public ICollection<Match> HomeMatches { get; set; } = new List<Match>();
        public ICollection<Match> AwayMatches { get; set; } = new List<Match>();
    }
}
