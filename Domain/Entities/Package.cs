using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Domain.Entities
{
    public class Package
    {
        public Guid Id { get; set; }

        public string Serves { get; set; }
        public string ServicePresentation { get; set; }
        public string SetupTime { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string Name { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string Description { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public int Price { get; set; }
        public List<PackageRequirement> PackageRequirements { get; set; }
        public List<PackageOption> PackageOptions { get; set; }
        public List<Menu> Menus { get; set; }
        public Guid CateringId { get; set; }
        [ForeignKey(nameof(CateringId))]
        public Catering Catering { get; set; }
    }
}
