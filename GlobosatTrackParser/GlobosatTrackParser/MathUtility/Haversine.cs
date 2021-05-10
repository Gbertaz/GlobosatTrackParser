// MIT License
//
// Copyright(c) 2021 Giovanni Bertazzoni <nottheworstdev@gmail.com>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;

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