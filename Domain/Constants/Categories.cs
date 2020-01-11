using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Domain.Constants
{
    public static class Categories
    {
        public const string Dessert = "dessert";
        public const string Office = "office";
        public const string Wedding = "wedding";
        public const string Birthday = "birthday";
        public const string HiTea = "hitea";
        public const string Buffet = "buffet";
        public const string Station = "station";

        public static readonly List<string> CategoriesList = new List<string>{ Dessert, Office, Wedding, Birthday, HiTea, Buffet, Station };
    }
}
