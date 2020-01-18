using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Contracts.V1.Requests
{
    public class UpdatePackageOptionItemRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
