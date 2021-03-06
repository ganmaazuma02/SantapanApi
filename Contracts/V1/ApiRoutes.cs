﻿using System;
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
            public const string RegisterCustomer = Base + "/account/register/customer";
            public const string RegisterCaterer = Base + "/account/register/caterer";
            public const string RegisterAdmin = Base + "/account/register/admin";
            public const string Upgrade = Base + "/account/upgrade";
            public const string Login = Base + "/account/login";
            public const string Refresh = Base + "/account/refresh";
            public const string Me = Base + "/account/me";
        }

        public static class Caterings
        {
            public const string GetAll = Base + "/caterings";
            public const string Get = Base + "/caterings/{cateringId}";
            public const string Update = Base + "/caterings/{cateringId}";
            public const string Delete = Base + "/caterings/{cateringId}";
            public const string Create = Base + "/caterings";
        }

        public static class Packages
        {
            public const string GetPackagesForOneCatering = Base + "/caterings/{cateringId}/packages";
            public const string GetAll = Base + "/packages";
            public const string Get = Base + "/packages/{packageId}";
            public const string Update = Base + "/caterings/{cateringId}/packages/{packageId}";
            public const string Delete = Base + "/packages/{packageId}";
            public const string CreatePackageForOneCatering = Base + "/caterings/{cateringId}/packages";
        }

        public static class Users
        {
            public const string GetAll = Base + "/users";
            public const string Get = Base + "/users/{userId}";
            public const string Update = Base + "/users/{userId}";
            public const string Delete = Base + "/users/{userId}";
            public const string Create = Base + "/users";
        }
    }
}
