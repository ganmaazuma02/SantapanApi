using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Contracts.V1.Responses
{
    public class PackageRequirementResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
