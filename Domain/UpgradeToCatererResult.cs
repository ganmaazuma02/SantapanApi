using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Domain
{
    public class UpgradeToCatererResult
    {
        public bool Success { get; set; }

        public IEnumerable<string> Errors { get; set; }

        public string UserId { get; set; }
    }
}
