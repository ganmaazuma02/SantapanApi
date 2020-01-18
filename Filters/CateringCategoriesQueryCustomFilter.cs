using SantapanApi.Domain.Entities;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Filters
{
    public class CateringCategoriesQueryCustomFilter : ISieveCustomFilterMethods
    {
        public IQueryable<Catering> CategoryIsAnyOf(IQueryable<Catering> source, string op, string[] values)
        {
            var result = source.Where(c => c.CateringCategories.Any(cc => values.Contains(cc.Category.Name)));
            return result;
        }
    }
}
