﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Domain.Entities
{
    public class PackageOptionItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public Guid PackageOptionId { get; set; }

        [ForeignKey(nameof(PackageOptionId))]
        public PackageOption PackageOption { get; set; }
    }
}
