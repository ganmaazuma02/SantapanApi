using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Domain.Constants
{
    public static class CateringSession
    {
        public const string Morning = "morning";
        public const string Afternoon = "afternoon";
        public const string Night = "night";

        public static readonly List<string> sessionsList = new List<string> { Morning, Afternoon, Night };

    }
}
