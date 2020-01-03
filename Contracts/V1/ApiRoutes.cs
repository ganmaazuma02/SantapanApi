using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Contracts.V1
{
    public static class ApiRoutes
    {
        public const string Root = "api";

        public const string Version = "v1";

        public const string Base = Root + "/" + Version;

        public static class Info
        {
            public const string GetInfo = Base + "/info";
        }

        public static class Account
        {
            public const string Register = Base + "/account/register";
            public const string Login = Base + "/account/login";
            public const string Me = Base + "/account/me";
        }
    }
}
