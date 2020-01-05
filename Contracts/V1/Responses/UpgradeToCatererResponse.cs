using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Contracts.V1.Responses
{
    public class UpgradeToCatererResponse
    {
        public Guid CateringId { get; set; }
        public string UserId { get; set; }
    }
}
