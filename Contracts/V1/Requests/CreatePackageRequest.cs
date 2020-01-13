using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Contracts.V1.Requests
{
    public class CreatePackageRequest
    {
        [Required]
        public string Serves { get; set; }
        [Required]
        public string ServicePresentation { get; set; }
        [Required]
        public string SetupTime { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Price { get; set; }
        public string[] PackageRequirements { get; set; }
        public IEnumerable<CreatePackageOptionRequest> PackageOptions { get; set; }
        [Required]
        public string[] Menus { get; set; }
    }
}
