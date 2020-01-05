using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Contracts.V1.Requests
{
    public class CreateCateringRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Details { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
