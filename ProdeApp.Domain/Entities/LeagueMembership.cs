﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdeApp.Domain.Entities
{
    public class LeagueMembership
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int LeagueId { get; set; }
        public League League { get; set; } = null!;

        public int MembershipId { get; set; }
        public DateTime JoinedAt { get; set; }
    }

}
