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

namespace GlobosatTrackParser.Statistics
{
    public class FixCounter
    {
        public int FixesLostNumber { get; private set; }
        public int FixesTotalNumber { get; private set; }

        private int _fixesInOneSecond;
        private int _gpsUpdateRate;      //hertz

        private TimeSpan _previousFixTime;
        private TimeSpan _firstFixTime;

        public FixCounter(int gpsUpdateRate)
        {
            _gpsUpdateRate = gpsUpdateRate;
            _fixesInOneSecond = 0;
            FixesLostNumber = 0;
            FixesTotalNumber = 0;
        }

        /// <summary>
        /// Counts the number of GPS fixes in a time interval of one second
        /// If it doesn't match the expected number of fixes according to the GPS update rate
        /// means we lost some data
        /// </summary>
        /// <param name="fix">GPS fix</param>
        public void Update(Models.GpsFix fix)
        {
            //Save the timestamp of the first fix
            if (FixesTotalNumber == 0) { _firstFixTime = _previousFixTime = fix.Time; }
            FixesTotalNumber++;

            //The very first second of logging is discarded because it may not be stable
            if (IsSameSecond(fix.Time, _firstFixTime)) return;

            //Increments the count until a new second starts
            if (IsSameSecond(fix.Time, _previousFixTime)) { _fixesInOneSecond++; return; }

            //When a new second starts count the number of lost GPS fixes in the previous time frame
            if (!IsSameSecond(_firstFixTime, _previousFixTime))
            {

                if (_gpsUpdateRate - _fixesInOneSecond > 0)
                {
                    //Console.WriteLine("Lost fix: " + (_gpsUpdateRate - _fixesInOneSecond));
                    //Console.WriteLine(fix.ToString());
                }

                FixesLostNumber += _gpsUpdateRate - _fixesInOneSecond;
            }

            _previousFixTime = fix.Time;
            _fixesInOneSecond = 1;
        }

        /// <summary>
        /// Compare the timestamp excluding the centiseconds
        /// </summary>
        /// <param name="currFixTime">Current GPS fix timestamp</param>
        /// <param name="prevFixTime">Previous GPS fix timestamp</param>
        /// <returns></returns>
        private bool IsSameSecond(TimeSpan currFixTime, TimeSpan prevFixTime)
        {
            return (currFixTime.Hours == prevFixTime.Hours &&
                currFixTime.Minutes == prevFixTime.Minutes &&
                currFixTime.Seconds == prevFixTime.Seconds);
        }

    }
}