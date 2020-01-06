using Microsoft.AspNetCore.Identity;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Domain
{
    public class Catering
    {
        public Guid Id { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string Name { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string Details { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string Category { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public DateTime CreatedAt { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public SantapanUser User { get; set; }
    }
}
