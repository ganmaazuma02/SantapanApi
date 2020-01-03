using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SantapanApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Data
{
    public class SantapanDbContext : IdentityDbContext
    {
        public SantapanDbContext(DbContextOptions<SantapanDbContext> options) : base(options)
        {

        }

        public DbSet<Catering> Caterings { get; set; }
    }
}
