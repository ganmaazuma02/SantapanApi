using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Contracts.V1.Responses
{
    public class ApiErrorResponse
    {
        public string ErrorType { get; set; }
        public int StatusCode { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
