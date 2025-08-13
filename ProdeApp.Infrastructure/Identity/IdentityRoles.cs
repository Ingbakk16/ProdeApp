using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdeApp.Infrastructure.Identity
{
    internal class IdentityRoles
    {
        public const string User = "User";
        public const string Admin = "Admin";

        public static List<string> GetAllRoles()
        {
            return new List<string>
            {
                User,
                Admin
            };
        }
        
           
    }
}
