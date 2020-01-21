using SantapanApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Extensions
{
    public static class PositionExtensions
    {
        public static double DistanceInKmTo(this Position pos1, Position pos2)
        {
            int EarthRadiusKm = 6371;

            var dLat = DegreesToRadian(pos2.Latitude - pos1.Latitude);
            var dLon = DegreesToRadian(pos2.Longitude - pos1.Longitude);

            var pos1LatRad = DegreesToRadian(pos1.Latitude);
            var pos2LatRad = DegreesToRadian(pos2.Latitude);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(pos1LatRad) * Math.Cos(pos2LatRad);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c;
        }

        public static double DegreesToRadian(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
