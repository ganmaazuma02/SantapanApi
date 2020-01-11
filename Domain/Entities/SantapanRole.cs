using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Domain.Entities
{
    public class SantapanRole : IdentityRole
    {
        public SantapanRole() : base()
        {

        }

        public SantapanRole(string roleName) : base(roleName)
        {

        }
    }
}
