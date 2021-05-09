using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobosatTrackParser.Models
{
    public class GpsFix
    {
        public double Latitude { get; }
        public double Longitude { get; }
        public int Satellites { get; }
        public float Speed { get; }
        public float AverageSpeed { get; set; }
        public float Altitude { get; }
        public float Heading { get; }
        public float Temperature { get; }
        public int HeartRate { get; }
        public DateTime Date { get; }
        public TimeSpan Time { get; }
        public TimeSpan TotalTime { get; set; }
        public TimeSpan TripTime { get; set; }
        public float LeanAngle { get; set; }
        public float Distance { get; set; }


        public GpsFix(string latitude,
            string longitude,
            string satellites,
            string speed,
            string altitude,
            string heading,
            string temperature,
            string bpm,
            string year,
            string month,
            string day,
            string hours,
            string minutes,
            string seconds,
            string centiseconds)
        {
            Latitude = string.IsNullOrWhiteSpace(latitude) ? 0.0f : StringToDouble(latitude);
            Longitude = string.IsNullOrWhiteSpace(longitude) ? 0.0f : StringToDouble(longitude);
            Satellites = string.IsNullOrWhiteSpace(satellites) ? 0 : Convert.ToInt32(satellites);
            Speed = string.IsNullOrWhiteSpace(speed) ? 0.0f : StringToFloat(speed);
            Altitude = string.IsNullOrWhiteSpace(altitude) ? 0.0f : StringToFloat(altitude);
            Heading = string.IsNullOrWhiteSpace(heading) ? 0.0f : StringToFloat(heading);
            Temperature = string.IsNullOrWhiteSpace(temperature) ? 0.0f : StringToFloat(temperature);
            HeartRate = string.IsNullOrWhiteSpace(bpm) ? 0 : Convert.ToInt32(bpm);
            LeanAngle = 0;
            Distance = 0;
            TotalTime = new TimeSpan(0, 0, 0);
            TripTime = new TimeSpan(0, 0, 0);
            AverageSpeed = 0;

            if (!string.IsNullOrWhiteSpace(year) && !string.IsNullOrWhiteSpace(month) && !string.IsNullOrWhiteSpace(day)
                 && !string.IsNullOrWhiteSpace(hours) && !string.IsNullOrWhiteSpace(minutes) && !string.IsNullOrWhiteSpace(seconds))
            {
                Date = new DateTime(
                    Convert.ToInt32(year)
                    , Convert.ToInt32(month)
                    , Convert.ToInt32(day)
                    , Convert.ToInt32(hours)
                    , Convert.ToInt32(minutes)
                    , Convert.ToInt32(seconds)
                    , DateTimeKind.Utc);

                Time = new TimeSpan(0,
                    Convert.ToInt32(hours)
                    , Convert.ToInt32(minutes)
                    , Convert.ToInt32(seconds)
                    , Convert.ToInt32(centiseconds) * 10);
            }
            //else
            //{
            //    Date = string.IsNullOrWhiteSpace(date) ? DateTime.MinValue : DateTime.SpecifyKind(DateTime.Parse(date), DateTimeKind.Utc);
            //    Time = string.IsNullOrWhiteSpace(time) ? TimeSpan.MinValue : TimeSpan.Parse(time);
            //}
        }

        public GpsFix(
            double latitude,
            double longitude,
            int satellites,
            float speed,
            float altitude,
            float heading,
            float temperature,
            int bpm,
            DateTime date,
            TimeSpan time)
        {
            Latitude = latitude;
            Longitude = longitude;
            Satellites = satellites;
            Speed = speed;
            Altitude = altitude;
            Heading = heading;
            Temperature = temperature;
            HeartRate = bpm;
            Date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            Time = time;
            LeanAngle = 0;
            Distance = 0;
            TotalTime = new TimeSpan(0, 0, 0);
            TripTime = new TimeSpan(0, 0, 0);
            AverageSpeed = 0;
        }

        private float StringToFloat(string textValue)
        {
            return float.Parse(textValue, CultureInfo.InvariantCulture.NumberFormat);
        }

        private double StringToDouble(string textValue)
        {
            return double.Parse(textValue, CultureInfo.InvariantCulture.NumberFormat);
        }

        public override string ToString()
        {
            //latitude,longitude,satellites,speed,altitude,heading,temperature,bpm,date,time,leanangle,distance,totaltime,triptime,averagespeed
            StringBuilder sb = new StringBuilder();
            sb.Append(FormatLatitude());
            sb.Append(",");
            sb.Append(FormatLongitude());
            sb.Append(",");
            sb.Append(Satellites);
            sb.Append(",");
            sb.Append(FormatSpeed());
            sb.Append(",");
            sb.Append(FormatAltitude());
            sb.Append(",");
            sb.Append(FormatHeading());
            sb.Append(",");
            sb.Append(FormatTemperature());
            sb.Append(",");
            sb.Append(FormatHeartRate());
            sb.Append(",");
            sb.Append(FormatDateToLocal());
            sb.Append(",");
            sb.Append(FormatTimeToLocal());
            sb.Append(",");
            sb.Append(FormatLeanAngle());
            sb.Append(",");
            sb.Append(FormatDistance());
            sb.Append(",");
            sb.Append(FormatTotalTime());
            sb.Append(",");
            sb.Append(FormatTripTime());
            sb.Append(",");
            sb.Append(FormatAverageSpeed());
            sb.AppendLine();

            return sb.ToString();
        }

        private string FormatLatitude()
        {
            return Latitude.ToString("0.0000000", CultureInfo.InvariantCulture.NumberFormat);
        }

        private string FormatLongitude()
        {
            return Longitude.ToString("0.0000000", CultureInfo.InvariantCulture.NumberFormat);
        }

        private string FormatAltitude()
        {
            return Altitude.ToString("0.0", CultureInfo.InvariantCulture.NumberFormat);
        }

        private string FormatTemperature()
        {
            return Temperature.ToString("0.0", CultureInfo.InvariantCulture.NumberFormat);
        }

        private string FormatHeartRate()
        {
            return HeartRate.ToString("0", CultureInfo.InvariantCulture.NumberFormat);
        }

        private string FormatHeading()
        {
            return Heading.ToString("0", CultureInfo.InvariantCulture.NumberFormat);
        }

        private string FormatSpeed()
        {
            return Speed.ToString("0.00", CultureInfo.InvariantCulture.NumberFormat);
        }

        private string FormatAverageSpeed()
        {
            return AverageSpeed.ToString("0.00", CultureInfo.InvariantCulture.NumberFormat);
        }

        private string FormatDateToLocal()
        {
            return Date.ToLocalTime().ToString("dd/MM/yyyy");
        }

        private string FormatTimeToLocal()
        {
            return Date.ToLocalTime().ToString("HH:mm:ss.fff");
        }

        private string FormatLeanAngle()
        {
            return LeanAngle.ToString("0", CultureInfo.InvariantCulture.NumberFormat);
        }

        private string FormatDistance()
        {
            return Distance.ToString("0.0", CultureInfo.InvariantCulture.NumberFormat);
        }

        private string FormatTotalTime()
        {
            return string.Format("{0}:{1}:{2}", TotalTime.Hours, TotalTime.Minutes, TotalTime.Seconds);
        }

        private string FormatTripTime()
        {
            return string.Format("{0}:{1}:{2}", TripTime.Hours, TripTime.Minutes, TripTime.Seconds);
        }

        public bool IsMoving()
        {
            return (Speed > 1.0);
        }

    }
}