using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Dtos
{
    public class CateringDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public string Category { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
