using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Domain.Entities
{
    public class SubPackage
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<PackageItem> PackageItems { get; set; }
    }
}
