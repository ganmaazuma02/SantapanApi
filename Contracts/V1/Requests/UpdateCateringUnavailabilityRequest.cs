using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Contracts.V1.Requests
{
    public class UpdateCateringUnavailabilityRequest
    {
        public DateTime UnavailableDate { get; set; }
        public string Session { get; set; }
    }
}
