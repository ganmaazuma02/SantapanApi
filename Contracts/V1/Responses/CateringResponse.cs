using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Contracts.V1.Responses
{
    public class CateringResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public List<string> Categories { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
