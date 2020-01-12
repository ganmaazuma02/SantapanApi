using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Dtos
{
    public class PackageDto
    {
        public Guid Id { get; set; }

        public string Serves { get; set; }
        public string ServicePresentation { get; set; }
        public string SetupTime { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Price { get; set; }
        public List<string> PackageRequirements { get; set; }
        public List<PackageOptionDto> PackageOptions { get; set; }
        public List<string> Menus { get; set; }
    }
}
