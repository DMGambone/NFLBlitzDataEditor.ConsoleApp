﻿using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;
using NFLBlitzDataEditor.Core.Readers;
using NFLBlitzDataEditor.Core.Models;
using NFLBlitzDataEditor.ConsoleApp.Extensions;

namespace NFLBlitzDataEditor.ConsoleApp
{
    class Program
    {
        static void ViewFileRange(Stream stream, uint startIndex, int count, uint segmentSize = 40)
        {
            stream.Seek(startIndex, SeekOrigin.Begin);
            byte[] buffer = new byte[count];
            stream.Read(buffer, 0, count);

            BinaryReader binaryReader = new BinaryReader(new MemoryStream(buffer));
            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
            {
                byte[] segment = binaryReader.ReadBytes((int)segmentSize);
                string segmentAsHex = String.Join(' ', segment.Select(b => b.ToString("x2")).ToArray());
                string segmentString = System.Text.Encoding.ASCII.GetString(segment.Select(b => (b >= 0x20 && b <= 0x7e) ? b : (byte)'.').ToArray());

                Console.WriteLine("{0, -" + ((segmentSize * 3) - 1).ToString() + "}  |  {1}", segmentAsHex, segmentString);
            }
        }

                static int FindInArray(int startIndex, byte[] find, byte[] buffer)
        {
            int findLength = find.Length;
            int bufferLength = buffer.Length;
            int position = startIndex;

            while (position + findLength + 1 < bufferLength)
            {
                int offset = 0;
                bool matched = true;

                while (offset < findLength && matched)
                {
                    matched = (find[offset] == buffer[position + offset]);
                    offset++;
                }

                if (matched)
                    return position;

                position++;
            }

            return -1;
        }

        static void ExtractImages(string dataFileName, DataFileSettings dataFileSettings)
        {
            using (System.IO.Stream stream = System.IO.File.OpenRead(dataFileName))
            {
                byte[] findPattern = new byte[] {
                   0x05, 0x80, 0x00, 0x00
                    };

                uint readBatchSize = 1024 * 1024 * 1024;
                uint position = 0;
                stream.Seek(position, SeekOrigin.Begin);
                List<uint> matches = new List<uint>();

                //Just focus on the inital 2GB of the data file
                byte[] buffer = new byte[readBatchSize];
                stream.Read(buffer, 0, (int)readBatchSize);

                int matchIndex = -1;
                while ((matchIndex = FindInArray(matchIndex + 1, findPattern, buffer)) != -1)
                {
                    matches.Add(position + (uint)matchIndex);
                }

                IDataFileReader reader = new Blitz2KArcadeDataFileReader(stream, dataFileSettings);
                foreach (uint match in matches)
                {
                    Image image = reader.ReadImage(match);
                    if (image == null)
                        continue;

                    image.SaveAsPNG(System.IO.Path.Combine(".\\images", $"{match.ToString("000000000")}.png"));
                }
            }
        }

        static void Main(string[] args)
        {
            string dataFileName = @"c:\mame\roms\blitz2k\blitz2k.bin";
            DataFileSettings dataFileSettings = new DataFileSettings
            {
                PlayerListOffset = 90483472,
                PlayerRecordSize = 92,
                PlayersPerTeam = 16,
                TeamListOffset = 90529104,
                TeamRecordSize = 116,
                TeamCount = 31
            };

            using (System.IO.Stream stream = System.IO.File.OpenRead(dataFileName))
            {
                IDataFileReader reader = new Blitz2KArcadeDataFileReader(stream, dataFileSettings);
                DataFile dataFile = reader.Read();

                IEnumerable<Team> teams = dataFile.Teams;
                foreach(Team team in teams)
                {
                    IEnumerable<Player> players = team.Players;

                    Console.WriteLine("------------------------------");
                    Console.WriteLine(team.ConvertToString());
                    foreach(Player player in players)
                    {
                        Console.WriteLine(player.ConvertToString());
                    }
                }
            }

            System.IO.Directory.CreateDirectory("images");
            ExtractImages(dataFileName, dataFileSettings);
        }
    }
}
