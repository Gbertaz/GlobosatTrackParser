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

            new TrackParser(gpsFrequency, args[1]);
            Console.ReadLine();
        }

    }
}