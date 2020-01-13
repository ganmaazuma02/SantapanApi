using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Contracts.V1.Requests
{
    public class CreatePackageOptionRequest
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string OptionsType { get; set; }
        [Required]
        public string[] PackageOptionItems { get; set; }
    }
}
