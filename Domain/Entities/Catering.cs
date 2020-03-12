using Microsoft.AspNetCore.Identity;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Domain.Entities
{
    public class Catering
    {
        public Guid Id { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string SSM { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string Details { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public DateTime CreatedAt { get; set; }

        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string State { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public List<CateringUnavailability> CateringUnavailabilities { get; set; }
        public List<CateringCategory> CateringCategories { get; set; }
        public List<Package> Packages { get; set; }


        [Sieve(CanSort = true, CanFilter = true)]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public SantapanUser User { get; set; }
    }
}
