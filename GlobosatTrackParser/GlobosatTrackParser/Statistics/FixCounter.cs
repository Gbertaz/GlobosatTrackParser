using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void PrintResult()
        {
            Console.WriteLine("\nTotal GPS fixes: {0}\tLost GPS fixes: {1}", FixesTotalNumber, FixesLostNumber);
        }

    }
}