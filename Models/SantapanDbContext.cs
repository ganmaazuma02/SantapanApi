using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Models
{
    public class SantapanDbContext : IdentityDbContext
    {
        public SantapanDbContext(DbContextOptions<SantapanDbContext> options) : base (options)
        {

        }

    }
}
