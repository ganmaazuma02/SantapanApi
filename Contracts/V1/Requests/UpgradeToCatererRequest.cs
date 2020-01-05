using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Contracts.V1.Requests
{
    public class UpgradeToCatererRequest
    {
        [Required]
        public string CateringName { get; set; }
        [Required]
        public string CateringDetails { get; set; }
        [Required]
        public string CateringCategory { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
