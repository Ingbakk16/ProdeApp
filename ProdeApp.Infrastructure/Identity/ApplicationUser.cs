using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProdeApp.Domain.Entities;
using ProdeApp.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace ProdeApp.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string DisplayName { get; set; } = null!;
        public string? AvatarKey { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }


        public virtual ICollection<League> OwnedLeagues { get; set; } = new List<League>();
        public virtual ICollection<LeagueMembership> LeagueMemberships { get; set; } = new List<LeagueMembership>();
        public virtual ICollection<Pick> Picks { get; set; } = new List<Pick>();

        public User ToDomainUser()
        {
            return new User
            {
                Id = this.Id, // Use inherited Id, not UserId
                Email = this.Email!,
                DisplayName = this.DisplayName,
                CreatedAt = this.CreatedAt,
                LastLoginAt = this.LastLoginAt
            };
        }

        public static ApplicationUser FromDomainUser(User user)
        {
            return new ApplicationUser
            {
                Id = user.Id,
                UserName = user.Email, // IdentityUser requires UserName
                Email = user.Email,
                DisplayName = user.DisplayName,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                EmailConfirmed = true
            };
        }
        public async Task<Role?> GetDomainRoleAsync(UserManager<ApplicationUser> userManager)
        {
            var roles = await userManager.GetRolesAsync(this);

            return roles.FirstOrDefault() switch
            {
                IdentityRoles.User => Role.User,
                IdentityRoles.Admin => Role.Admin,
                _ => null
            };
        }
       }
}

