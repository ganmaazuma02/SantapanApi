using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Contracts.V1.Responses
{
    public class PackageOptionResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string OptionsType { get; set; }
        public List<string> PackageOptionItems { get; set; }
    }
}
