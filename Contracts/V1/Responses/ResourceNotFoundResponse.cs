using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Contracts.V1.Responses
{
    public class ResourceNotFoundResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}
