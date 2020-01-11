using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Domain.Entities
{
    public class CateringCategory
    {
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public Guid CateringId { get; set; }
        public Catering Catering { get; set; }
    }
}
