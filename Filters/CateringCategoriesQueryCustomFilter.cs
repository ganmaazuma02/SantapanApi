using SantapanApi.Domain;
using SantapanApi.Domain.Entities;
using SantapanApi.Extensions;
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

        public IQueryable<Catering> WithinDistanceOf(IQueryable<Catering> source, string op, string[] values)
        {
            double lat;
            double lng;
            double withinDistance;

            if(double.TryParse(values[0], out lat))
            {
                if (double.TryParse(values[1], out lng))
                {
                    if(double.TryParse(values[2], out withinDistance))
                    {
                        Position pos = new Position { Latitude = lat, Longitude = lng };
                        var result = source.Where(c => new Position { Latitude = c.Latitude, Longitude = c.Longitude }.DistanceInKmTo(pos) < withinDistance);
                        return result;
                    }
                    
                }

            }

            return null;
        }
    }
}
