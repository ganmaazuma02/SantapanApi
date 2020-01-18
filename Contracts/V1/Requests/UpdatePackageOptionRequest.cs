using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Contracts.V1.Requests
{
    public class UpdatePackageOptionRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string OptionsType { get; set; }
        public IEnumerable<UpdatePackageOptionItemRequest> PackageOptionItems { get; set; }
    }
}
