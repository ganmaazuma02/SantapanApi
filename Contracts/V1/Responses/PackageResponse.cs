using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Contracts.V1.Responses
{
    public class PackageResponse
    {
        public Guid Id { get; set; }

        public string Serves { get; set; }
        public string ServicePresentation { get; set; }
        public string SetupTime { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Price { get; set; }
        public IEnumerable<PackageRequirementResponse> PackageRequirements { get; set; }
        public IEnumerable<PackageOptionResponse> PackageOptions { get; set; }
        public IEnumerable<MenuResponse> Menus { get; set; }
    }
}
