using SantapanApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Domain
{
    public class GetUserResult
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public string UserId { get; set; }

        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<Catering> Caterings { get; set; }
        public DateTime CreatedAt{ get; set; }
     
        public string Email { get; set; }
    }
}
