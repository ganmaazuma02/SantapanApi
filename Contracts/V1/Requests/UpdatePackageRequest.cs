using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Contracts.V1.Requests
{
    public class UpdatePackageRequest
    {
        [Required]
        public Guid Id { get; set; }
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
        public IEnumerable<UpdatePackageRequirementRequest> PackageRequirements { get; set; }
        public IEnumerable<UpdatePackageOptionRequest> PackageOptions { get; set; }
        [Required]
        public IEnumerable<UpdateMenuRequest> Menus { get; set; }
    }
}
