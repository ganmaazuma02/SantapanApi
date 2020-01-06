using Microsoft.AspNetCore.Identity;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Domain
{
    public class SantapanUser : IdentityUser
    {

        [Sieve(CanFilter = true, CanSort = true)]
        public string FirstName { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string LastName { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime CreatedAt { get; set; }
        public List<Catering> Caterings { get; set; }
    }
}
