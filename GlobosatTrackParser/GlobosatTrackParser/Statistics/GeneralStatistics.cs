using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private FixCounter fixCounter;
        private int _gpsUpdateRate;


        public GeneralStatistics(int gpsUpdateRate)
        {
            _gpsUpdateRate = gpsUpdateRate;
            fixCounter = new Statistics.FixCounter(gpsUpdateRate);
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
            fixCounter.Update(fix);

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


        public void PrintResult()
        {
            fixCounter.PrintResult();
            Console.WriteLine("");
            Console.WriteLine(
                "Speed MAX:\t\t{0} Km/h\n" +
                "Speed MIN:\t\t{1} Km/h\n" +
                "Speed AVG:\t\t{2} Km/h\n" +
                "Altitude MAX:\t\t{3} meters\n" +
                "Altitude MIN:\t\t{4} meters\n" +
                "Temperature MAX:\t{5} °C\n" +
                "Temperature MIN:\t{6} °C\n" +
                "Temperature AVG:\t{7} °C\n" +
                "Heart rate MAX:\t\t{8} bpm\n" +
                "Heart rate MIN:\t\t{9} bpm\n" +
                "Heart rate AVG:\t\t{10} bpm\n" +
                "Time total:\t\t{11}\n" +
                "Time moving:\t\t{12}\n" +
                "Distance:\t\t{13} Km\n",

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

        private string FormatTime(TimeSpan time)
        {
            return string.Format("{0}:{1}:{2}", time.Hours, time.Minutes, time.Seconds);
        }

    }
}