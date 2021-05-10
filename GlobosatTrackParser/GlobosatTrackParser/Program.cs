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
using System.IO;

namespace GlobosatTrackParser
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Invalid parameters, press ENTER to exit...");
                Console.ReadLine();
                return;
            }

            int gpsFrequency = 0;
            bool succeeded = int.TryParse(args[0], out gpsFrequency);

            if (succeeded == false || gpsFrequency <= 0)
            {
                Console.WriteLine("Gps frequency is not valid, please input an integer number greater than 0. Press ENTER to exit...");
                Console.ReadLine();
                return;
            }

            if (File.Exists(args[1]) == false)
            {
                Console.WriteLine(args[1]);
                Console.WriteLine("Input file not found, press ENTER to exit...");
                Console.ReadLine();
                return;
            }
            
            TrackParser parser = new TrackParser(gpsFrequency, args[1]);
            parser.ParseFile();

            //Prints the statistics
            Console.WriteLine(parser.GetStats().ToString());
            Console.ReadLine();
        }

    }
}