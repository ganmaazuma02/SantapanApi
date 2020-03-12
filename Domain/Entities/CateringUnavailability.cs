using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Domain.Entities
{
    public class CateringUnavailability
    {
        public Guid Id { get; set; }
        public DateTime UnavailableDate { get; set; }
        public string Session { get; set; }
        public Guid CateringId { get; set; }
        public Catering Catering { get; set; }
    }
}
