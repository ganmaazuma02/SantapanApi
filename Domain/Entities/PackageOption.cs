using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Domain.Entities
{
    public class PackageOption
    {
        public Guid Id { get; set; }
        public string OptionsType { get; set; }
        public List<PackageOptionItem> PackageOptionItems { get; set; }
    }
}
