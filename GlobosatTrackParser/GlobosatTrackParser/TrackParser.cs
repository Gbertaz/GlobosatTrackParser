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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GlobosatTrackParser
{
    public class TrackParser
    {
        private const char SEPARATOR = ',';
        private const string FIX_LOST_CHECKMARK = "****FIX_LOST****";

        private string _inputFilePath;
        private int _gpsUpdateRate;
        private bool _processLostFix;

        private List<Models.GpsFix> _gpsFixes;
        private List<string> _headerColumns;

        private Statistics.GeneralStatistics _generalStats;


        public TrackParser(int gpsUpdateRate, string inputFilePath)
        {
            _gpsUpdateRate = gpsUpdateRate;
            _inputFilePath = inputFilePath;
            _processLostFix = false;

            _gpsFixes = new List<Models.GpsFix>();
            _generalStats = new Statistics.GeneralStatistics(gpsUpdateRate);
        }


        /// <summary>
        /// Reads the input file line by line
        /// </summary>
        public void ParseFile()
        {
            _gpsFixes.Clear();
            _generalStats.Reset();

            const Int32 bufferSize = 128;
            using (var fileStream = File.OpenRead(_inputFilePath))
            {
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, bufferSize))
                {
                    String line;
                    bool parseHeader = true;

                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (parseHeader)
                        {
                            parseHeader = false;
                            ParseHeader(line);
                        }
                        else
                        {
                            ParseRow(line);
                        }
                    }
                }
                fileStream.Close();
            }

            WriteOutput();
        }

        /// <summary>
        /// Writes the processed data to the output file
        /// </summary>
        private void WriteOutput()
        {
            try
            {
                string outputFilePath = Path.Combine(Path.GetDirectoryName(_inputFilePath),
                Path.GetFileNameWithoutExtension(_inputFilePath) + "_out" + Path.GetExtension(_inputFilePath));
                FileStream outputStream = File.OpenWrite(outputFilePath);

                //Writes the statistics
                WriteToFile(outputStream, _generalStats.ToString());

                //Writes the header
                string header = string.Join(SEPARATOR.ToString(), _headerColumns);
                WriteToFile(outputStream, header);

                //Writes the data
                foreach (Models.GpsFix fix in _gpsFixes)
                {
                    WriteToFile(outputStream, fix.ToString());
                }

                outputStream.Flush();
                outputStream.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Writes a string to the output stream
        /// </summary>
        /// <param name="outputStream">Output stream</param>
        /// <param name="text">Text to be written</param>
        private void WriteToFile(FileStream outputStream, string text)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(text);
            outputStream.Write(info, 0, info.Length);
        }

        /// <summary>
        /// Parses the header of the input file
        /// </summary>
        /// <param name="headerText">Header text</param>
        private void ParseHeader(string headerText)
        {
            _headerColumns = new List<string>();
            _headerColumns.AddRange(headerText.Split(SEPARATOR));

            //Add the header columns of the calculated values
            _headerColumns.Add("leanangle");
            _headerColumns.Add("distance");
            _headerColumns.Add("totaltime");
            _headerColumns.Add("triptime");
            _headerColumns.Add("averagespeed" + System.Environment.NewLine);
        }

        /// <summary>
        /// Parses the text row
        /// </summary>
        /// <param name="text"></param>
        public void ParseRow(string text)
        {
            //Previous gps fix
            Models.GpsFix prevFix = (_gpsFixes.Count == 0) ? null : _gpsFixes.Last();

            //If the fix lost checkmark is detected means that the next text line is the first fix after a gps signal loss
            if (text.Contains(FIX_LOST_CHECKMARK))
            {
                _processLostFix = true;
                return;
            }

            //Values of the data row
            string[] values = text.Split(SEPARATOR);

            //Current fix
            Models.GpsFix fix = new Models.GpsFix(
                values[0],
                values[1],
                values[2],
                values[3],
                values[4],
                values[5],
                values[6],
                values[7],
                values[8],
                values[9],
                values[10],
                values[11],
                values[12],
                values[13],
                values[14]);

            //Generates the lost fixes
            if (_processLostFix)
            {
                _processLostFix = false;

                MathUtility.LostFixesFiller filler = new MathUtility.LostFixesFiller(prevFix, fix, _gpsUpdateRate);
                List<Models.GpsFix> lostFixes = filler.GenerateLostFixes();

                for (int i = 0; i < lostFixes.Count; i++)
                {
                    Models.GpsFix item = lostFixes[i];
                    if (i == 0) ComputeCalculatedValues(prevFix, item);
                    else ComputeCalculatedValues(lostFixes[i - 1], item);
                }
            }
            else
            {
                ComputeCalculatedValues(prevFix, fix);
            }
        }

        /// <summary>
        /// Computes the calculated values
        /// </summary>
        /// <param name="prevFix">Previous GPS fix</param>
        /// <param name="currentFix">Current GPS fix</param>
        private void ComputeCalculatedValues(Models.GpsFix prevFix, Models.GpsFix currentFix)
        {
            //Computes general statistics
            _generalStats.Update(currentFix, prevFix);

            //Set the calculated values
            if (prevFix != null)
            {
                if (currentFix.IsMoving() && prevFix.IsMoving())
                    currentFix.LeanAngle = (int)MathUtility.LeanAngleMath.Calculate(currentFix.Heading, prevFix.Heading, currentFix.Speed, prevFix.Speed, _gpsUpdateRate);
            }
            currentFix.Distance = (float)_generalStats.TotalTripDistance;
            currentFix.TotalTime = _generalStats.TotalTime;
            currentFix.TripTime = _generalStats.TotalTripTime;

            _gpsFixes.Add(currentFix);
        }

        /// <summary>
        /// Returns the statistics object
        /// </summary>
        /// <returns></returns>
        public Statistics.GeneralStatistics GetStats()
        {
            return _generalStats;
        }

    }
}