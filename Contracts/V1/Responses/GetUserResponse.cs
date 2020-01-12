
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Contracts.V1.Responses
{
    public class GetUserResponse
    {
        public string UserId { get; set; }

        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<CateringResponse> Caterings { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Email { get; set; }
    }
}
