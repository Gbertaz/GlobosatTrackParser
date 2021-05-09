using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobosatTrackParser.MathUtility
{
    public static class Haversine
    {
        /// <summary>
        /// Calculates the great-circle distance between the points A and B.
        /// Doesn't take into account the altitude
        /// </summary>
        /// <param name="latitudeA">Latitude of point A</param>
        /// <param name="longitudeA">Longitude of point A</param>
        /// <param name="latitudeB">Latitude of point B</param>
        /// <param name="longitudeB">Longitude of point B</param>
        /// <returns>Distance in Km between the points represented by the given coordinates</returns>
        public static double Calculate(double latitudeA, double longitudeA, double latitudeB, double longitudeB)
        {
            var dLat = Converters.DegreesToRadians(latitudeB - latitudeA);
            var dLon = Converters.DegreesToRadians(longitudeB - longitudeA);
            latitudeA = Converters.DegreesToRadians(latitudeA);
            latitudeB = Converters.DegreesToRadians(latitudeB);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(latitudeA) * Math.Cos(latitudeB);
            return 2 * Constants.EARTH_RADIUS * Math.Asin(Math.Sqrt(a));
        }
    }

}