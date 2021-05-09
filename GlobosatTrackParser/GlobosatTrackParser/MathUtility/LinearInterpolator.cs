using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobosatTrackParser.MathUtility
{
    public class LinearInterpolator
    {
        public static double[] Compute(double[] source, int outputLength)
        {
            double[] destination = new double[outputLength];

            destination[0] = source[0];
            int jPrevious = 0;
            for (int i = 1; i < source.Length; i++)
            {
                int j = i * (destination.Length - 1) / (source.Length - 1);
                Interpolate(destination, jPrevious, j, source[i - 1], source[i]);

                jPrevious = j;
            }

            return destination;
        }

        private static void Interpolate(double[] destination, int destFrom, int destTo, double valueFrom, double valueTo)
        {
            int destLength = destTo - destFrom;
            double valueLength = valueTo - valueFrom;
            for (int i = 0; i <= destLength; i++)
                destination[destFrom + i] = valueFrom + (valueLength * i) / destLength;
        }

    }
}