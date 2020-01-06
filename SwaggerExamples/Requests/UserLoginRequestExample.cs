using SantapanApi.Contracts.V1.Requests;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.SwaggerExamples.Requests
{
    public class UserLoginRequestExample : IExamplesProvider<UserLoginRequest>
    {
        public UserLoginRequest GetExamples()
        {
            return new UserLoginRequest
            {
                Email = "admin@santapan.my",
                Password = "@Abc123"
            };
        }
    }
}
