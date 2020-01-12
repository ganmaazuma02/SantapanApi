using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Dtos
{
    public class PackageOptionDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string OptionsType { get; set; }
        public List<string> PackageOptionItems { get; set; }
    }
}
