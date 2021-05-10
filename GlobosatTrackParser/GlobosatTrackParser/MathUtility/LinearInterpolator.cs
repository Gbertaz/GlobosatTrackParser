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