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

/// https://hackaday.io/project/13438-motorcycle-lean-angle-video-overlay/log/45115-lean-angle-calculation-method#discussion-list
namespace GlobosatTrackParser.MathUtility
{
    public static class LeanAngleMath
    {
        /// <summary>
        /// Estimates the lean angle
        /// </summary>
        /// <param name="currentHeading"></param>
        /// <param name="previousHeading"></param>
        /// <param name="currentSpeed"></param>
        /// <param name="previousSpeed"></param>
        /// <param name="gpsUpdateRate"></param>
        /// <returns></returns>
        public static float Calculate(float currentHeading, float previousHeading, float currentSpeed, float previousSpeed, int gpsUpdateRate)
        {
            //The value of turn will vary from 1 and -1.
            //> 0 right turn
            //< 0 left turn
            double turnDirection = ComputeTurn(currentHeading, previousHeading);

            double yaw_delta = ComputeYawRate(currentHeading, previousHeading, turnDirection);

            double period = ComputeTurningPeriod(yaw_delta, gpsUpdateRate);

            //Average speed between the two data points
            double avgSpeed = (currentSpeed + previousSpeed) / 2;

            double radius = ComputeTurnRadius(avgSpeed, period);

            float leanAngle = (float)(Math.Atan(Math.Pow(avgSpeed, 2) / (radius * Constants.STANDARD_GRAVITY)) * Constants.DEG_PER_RAD);

            return leanAngle;
        }

        /// <summary>
        /// Determine if is a left or right turn
        /// </summary>
        /// <param name="currHeading"></param>
        /// <param name="prevHeading"></param>
        /// <returns></returns>
        private static double ComputeTurn(float currHeading, float prevHeading)
        {
            return (Math.Cos(prevHeading * Constants.RAD_PER_DEG) * Math.Sin(currHeading * Constants.RAD_PER_DEG) - Math.Sin(prevHeading * Constants.RAD_PER_DEG) * Math.Cos(currHeading * Constants.RAD_PER_DEG));
        }

        /// <summary>
        /// Computes the rate of change of direction
        /// </summary>
        /// <param name="currHeading"></param>
        /// <param name="prevHeading"></param>
        /// <param name="turnDirection"></param>
        /// <returns></returns>
        private static double ComputeYawRate(float currHeading, float prevHeading, double turnDirection)
        {
            double yaw_delta = currHeading - prevHeading;

            if (Math.Abs(yaw_delta) > 180)
            {
                yaw_delta = turnDirection * (360 - Math.Abs(yaw_delta));
            }

            return yaw_delta;
        }

        /// <summary>
        /// Computes the period of the turn
        /// </summary>
        /// <param name="yaw"></param>
        /// <param name="gpsUpdateRate"></param>
        /// <returns></returns>
        private static double ComputeTurningPeriod(double yaw, int gpsUpdateRate)
        {
            //Period of time between samples. 
            //10Hz => 0,1 seconds
            //5Hz  => 0,2 seconds
            return (360 / (yaw * Converters.HertzToSeconds(gpsUpdateRate)));
        }

        /// <summary>
        /// Computes the radius of the turn
        /// </summary>
        /// <param name="speed">Speed in Km/h</param>
        /// <param name="period"></param>
        /// <returns></returns>
        private static double ComputeTurnRadius(double speed, double period)
        {
            return (Converters.ConvertKmhToMs(speed) * (period / (Math.PI * 2)));
        }

    }
}