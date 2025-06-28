using ProdeApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdeApp.Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? AvatarKey { get; set; }
        public Role Role { get; set; }

        public ICollection<League> OwnedLeagues { get; set; } = new List<League>();
        public ICollection<LeagueMembership> LeagueMemberships { get; set; } = new List<LeagueMembership>();
        public ICollection<Pick> Picks { get; set; } = new List<Pick>();
    }

}
