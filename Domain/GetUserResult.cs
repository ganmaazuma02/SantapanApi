using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Domain
{
    public class GetUserResult
    {
        public bool Success { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
    }
}
