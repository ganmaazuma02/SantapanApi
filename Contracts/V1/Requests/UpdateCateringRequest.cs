using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Contracts.V1.Requests
{
    public class UpdateCateringRequest
    {
        public string Name { get; set; }
        public string Details { get; set; }

        public string[] CateringCategories { get; set; }
        public IEnumerable<UpdateCateringUnavailabilityRequest> CateringUnavailabilities { get; set; }
    }
}
