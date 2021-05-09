using System;

namespace GlobosatTrackParser.MathUtility
{
    public static class Converters
    {
        /// <summary>
        /// Converts the frequency from Hertz to seconds 
        /// </summary>
        /// <param name="freqHertz">Frequency in Hertz</param>
        /// <returns>Frequency in seconds</returns>
        public static float HertzToSeconds(int freqHertz)
        {
            //10Hz => 0,1 seconds
            //5Hz  => 0,2 seconds
            return 1 / freqHertz;
        }

        /// <summary>
        /// Converts the frequency from Hertz to milliseconds
        /// </summary>
        /// <param name="freqHertz">Frequency in Hertz</param>
        /// <returns>Frequency in milliseconds</returns>
        public static int HertzToMilliseconds(int freqHertz)
        {
            //10Hz => 100 milliseconds
            //5Hz  => 200 milliseconds
            return 1000 / freqHertz;
        }

        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        /// <param name="angle">Angle in degrees</param>
        /// <returns>Radians</returns>
        public static double DegreesToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        /// <summary>
        /// Converts the speed from Km/h to m/s
        /// </summary>
        /// <param name="speedKmh">Speed in Km/h</param>
        /// <returns>Speed in meters per second</returns>
        public static double ConvertKmhToMs(double speedKmh)
        {
            return speedKmh / 3.6;
        }

        /// <summary>
        /// Converts the speed from m/s to km/h
        /// </summary>
        /// <param name="speedMs">Spped in m/s</param>
        /// <returns>Speed in km/h</returns>
        public static double ConvertMsToKmh(double speedMs)
        {
            return speedMs * 3.6;
        }

    }
}