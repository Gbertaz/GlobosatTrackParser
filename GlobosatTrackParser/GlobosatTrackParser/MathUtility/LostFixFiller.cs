using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobosatTrackParser.MathUtility
{
    public class LostFixesFiller
    {
        private Models.GpsFix _beforeFixLost;
        private Models.GpsFix _afterFixLost;
        private int _gpsUpdateRate;

        public LostFixesFiller(Models.GpsFix beforeFixLost, Models.GpsFix afterFixLost, int gpsUpdateRate)
        {
            _beforeFixLost = beforeFixLost;
            _afterFixLost = afterFixLost;
            _gpsUpdateRate = gpsUpdateRate;
        }

        public List<Models.GpsFix> GenerateLostFixes()
        {
            List<Models.GpsFix> result = new List<Models.GpsFix>();

            //Milliseconds delay between each GPS fix
            TimeSpan tick = new TimeSpan(0, 0, 0, 0, Converters.HertzToMilliseconds(_gpsUpdateRate));
            TimeSpan timestamp = _beforeFixLost.Time.Add(tick);

            int countMissingFix = 0;
            while (timestamp != _afterFixLost.Time)
            {
                countMissingFix++;
                timestamp = timestamp.Add(tick);
            }

            //Interpolate the missing values
            double[] lerpLatitude = InterpolateValues(_beforeFixLost.Latitude, _afterFixLost.Latitude, countMissingFix);
            double[] lerpLongitude = InterpolateValues(_beforeFixLost.Longitude, _afterFixLost.Longitude, countMissingFix);
            double[] lerpSpeed = InterpolateValues(_beforeFixLost.Speed, _afterFixLost.Speed, countMissingFix);
            double[] lerpAltitude = InterpolateValues(_beforeFixLost.Altitude, _afterFixLost.Altitude, countMissingFix);
            double[] lerpHeading = InterpolateValues(_beforeFixLost.Heading, _afterFixLost.Heading, countMissingFix);

            int index = 0;
            timestamp = _beforeFixLost.Time.Add(tick);
            while (timestamp != _afterFixLost.Time)
            {
                DateTime date = new DateTime(_beforeFixLost.Date.Year,
                    _beforeFixLost.Date.Month,
                    _beforeFixLost.Date.Day,
                    timestamp.Hours,
                    timestamp.Minutes,
                    timestamp.Seconds,
                    timestamp.Milliseconds);

                //New fix
                Models.GpsFix fix = new Models.GpsFix(lerpLatitude[index],
                    lerpLongitude[index],
                    0,
                    (float)lerpSpeed[index],
                    (float)lerpAltitude[index],
                    (float)lerpHeading[index],
                    _afterFixLost.Temperature,
                    0,
                    date,
                    timestamp);

                result.Add(fix);

                timestamp = timestamp.Add(tick);

                index++;
            }

            result.Add(_afterFixLost);

            return result;
        }

        private double[] InterpolateValues(double initialValue, double endValue, int outputLength)
        {
            double[] source = { initialValue, endValue };
            return LinearInterpolator.Compute(source, outputLength);
        }

    }
}