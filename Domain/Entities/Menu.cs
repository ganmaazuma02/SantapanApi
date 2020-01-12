using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Domain.Entities
{
    public class Menu
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid PackageId { get; set; }

        [ForeignKey(nameof(PackageId))]
        public Package Package { get; set; }

        //public List<MenuItem> MenuItems { get; set; }
    }
}
