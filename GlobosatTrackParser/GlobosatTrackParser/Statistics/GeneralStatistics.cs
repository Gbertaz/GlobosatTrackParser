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
    public class GeneralStatistics
    {
        public float MaxSpeed { get; private set; }
        public float MinSpeed { get; private set; }
        public float AverageSpeed { get; set; }
        public float MaxAltitude { get; private set; }
        public float MinAltitude { get; private set; }
        public float MaxLeanAngleRight { get; private set; }
        public float MaxLeanAngleLeft { get; private set; }
        public float MaxLeanAngle { get; private set; }
        public float MaxTemperature { get; private set; }
        public float MinTemperature { get; private set; }
        public float AverageTemperature { get; private set; }
        public int MaxHeartRate { get; private set; }
        public int MinHeartRate { get; private set; }
        public int AverageHeartRate { get; private set; }
        public TimeSpan TotalTime { get; private set; }
        public TimeSpan TotalTripTime { get; private set; }
        public double TotalTripDistance { get; private set; }

        private FixCounter _fixCounter;
        private int _gpsUpdateRate;


        public GeneralStatistics(int gpsUpdateRate)
        {
            _gpsUpdateRate = gpsUpdateRate;
            _fixCounter = new Statistics.FixCounter(gpsUpdateRate);
            Reset();
        }

        public void Reset()
        {
            MaxSpeed = float.MinValue;
            MinSpeed = float.MaxValue;
            AverageSpeed = 0;
            MaxAltitude = float.MinValue;
            MinAltitude = float.MaxValue;
            MaxLeanAngleRight = float.MinValue;
            MaxLeanAngleLeft = float.MaxValue;
            MaxLeanAngle = float.MinValue;
            MaxTemperature = float.MinValue;
            MinTemperature = float.MaxValue;
            AverageTemperature = 0;
            MaxHeartRate = int.MinValue;
            MinHeartRate = int.MaxValue;
            AverageHeartRate = 0;
            TotalTripDistance = 0;
            TotalTime = new TimeSpan(0, 0, 0);
            TotalTripTime = new TimeSpan(0, 0, 0);
        }

        /// <summary>
        /// Calculates the statistics
        /// </summary>
        /// <param name="fix">GPS fix</param>
        /// <param name="prevFix">Previous GPS fix</param>
        public void Update(Models.GpsFix fix, Models.GpsFix prevFix)
        {
            if (fix.Speed > MaxSpeed) MaxSpeed = fix.Speed;
            if (fix.Speed < MinSpeed) MinSpeed = fix.Speed;
            if (fix.Altitude > MaxAltitude) MaxAltitude = fix.Altitude;
            if (fix.IsMoving() && fix.Altitude < MinAltitude && fix.Altitude != 0.0f) MinAltitude = fix.Altitude;
            if (Math.Abs(fix.LeanAngle) > MaxLeanAngle) MaxLeanAngle = Math.Abs(fix.LeanAngle);
            if (fix.LeanAngle > MaxLeanAngleRight) MaxLeanAngleRight = fix.LeanAngle;
            if (fix.LeanAngle < MaxLeanAngleLeft) MaxLeanAngleLeft = fix.LeanAngle;
            if (fix.HeartRate > MaxHeartRate) MaxHeartRate = fix.HeartRate;
            if (fix.HeartRate < MinHeartRate) MinHeartRate = fix.HeartRate;
            if (fix.Temperature > MaxTemperature) MaxTemperature = fix.Temperature;
            if (fix.Temperature < MinTemperature) MinTemperature = fix.Temperature;

            //Total fixes and lost fixes counter
            _fixCounter.Update(fix);

            if (prevFix == null) return;

            //Total time
            TimeSpan timeDiff = fix.Time - prevFix.Time;
            TotalTime += timeDiff;

            //Filtering out the fix when the GPS is not moving to reduce the measurement error
            if (fix.IsMoving() && prevFix.IsMoving())
            {
                //Computes total trip time (only the distance in motion)
                TotalTripTime += timeDiff;

                //Computes lean angle
                fix.LeanAngle = (int)MathUtility.LeanAngleMath.Calculate(fix.Heading, prevFix.Heading, fix.Speed, prevFix.Speed, _gpsUpdateRate);

                //Calculates the distance between two coordinates using Haversine formula
                TotalTripDistance += MathUtility.Haversine.Calculate(fix.Latitude, fix.Longitude, prevFix.Latitude, prevFix.Longitude);
            }

            //Average speed (takes into account only the moving time)
            if (TotalTripTime.TotalHours != 0)
            {
                AverageSpeed = (float)(TotalTripDistance / TotalTripTime.TotalHours);
                fix.AverageSpeed = AverageSpeed;
            }
        }

        public override string ToString()
        {
            return string.Format(
                "Total GPS fixes:\t{0}" + NewLine() +
                "Lost GPS fixes:\t\t{1}" + NewLine() +
                "Speed MAX:\t\t{2} Km/h" + NewLine() +
                "Speed MIN:\t\t{3} Km/h" + NewLine() +
                "Speed AVG:\t\t{4} Km/h" + NewLine() +
                "Altitude MAX:\t\t{5} meters" + NewLine() +
                "Altitude MIN:\t\t{6} meters" + NewLine() +
                "Temperature MAX:\t{7} °C" + NewLine() +
                "Temperature MIN:\t{8} °C" + NewLine() +
                "Temperature AVG:\t{9} °C" + NewLine() +
                "Heart rate MAX:\t\t{10} bpm" + NewLine() +
                "Heart rate MIN:\t\t{11} bpm" + NewLine() +
                "Heart rate AVG:\t\t{12} bpm" + NewLine() +
                "Time total:\t\t{13}" + NewLine() +
                "Time moving:\t\t{14}" + NewLine() +
                "Distance:\t\t{15} Km" + NewLine(),
                _fixCounter.FixesTotalNumber,
                _fixCounter.FixesLostNumber,
                MaxSpeed.ToString("0.0"),
                MinSpeed.ToString("0.0"),
                AverageSpeed.ToString("0.0"),
                MaxAltitude,
                MinAltitude,
                MaxTemperature.ToString("0.0"),
                MinTemperature.ToString("0.0"),
                AverageTemperature.ToString("0.0"),
                MaxHeartRate,
                MinHeartRate,
                AverageHeartRate,
                FormatTime(TotalTime),
                FormatTime(TotalTripTime),
                TotalTripDistance.ToString("0.0"));
        }

        private string NewLine() { return Environment.NewLine; }

        private string FormatTime(TimeSpan time)
        {
            return string.Format("{0}:{1}:{2}", time.Hours, time.Minutes, time.Seconds);
        }

    }
}