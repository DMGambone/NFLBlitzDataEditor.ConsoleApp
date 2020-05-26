using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using NFLBlitzDataEditor.Core.Readers;
using NFLBlitzDataEditor.Core.Models;
using NFLBlitzDataEditor.ConsoleApp.Extensions;
using NFLBlitzDataEditor.ConsoleApp.Helpers;


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

        static void Main(string[] args)
        {
            string dataFileName = @"C:\development\NFLBlitzDataEditor\Data Files\Blitz2kGold-arcade.bin";

            using (System.IO.Stream stream = System.IO.File.OpenRead(dataFileName))
            {
                IDataFileReader reader = new NFLBlitzDataEditor.Core.Readers.Blitz2kArcade.Blitz2kArcadeDataFileReader(stream);
                DataFile dataFile = reader.Read();

                IEnumerable<Team> teams = dataFile.Teams;
                foreach (Team team in teams)
                {
                    IEnumerable<Player> players = team.Players;

                    Console.WriteLine("------------------------------");
                    Console.WriteLine(team.ConvertToString());
                    foreach (Player player in players)
                    {
                        Console.WriteLine(player.ConvertToString());
                    }
                }
            }

            System.IO.Directory.CreateDirectory("images");
            NFLBlitzImageHelper imageHelper = new NFLBlitzImageHelper();
            imageHelper.ExtractAllImages(dataFileName, "images", true);

            // Image image = null;
            // //Test: Read TMSEL00.WMS (located at 91313848).  It's equivalent image is located at 15005700.
            // Console.WriteLine();
            // image = imageHelper.GetImage(dataFileName, 91313848, 60, 15005700);
            // imageHelper.OutputImageInformation(image);

            // //Test: Read TMSEL00.WMS (located at 91316020).  It's equivalent image is located at 15140868.
            // Console.WriteLine();
            // image = imageHelper.GetImage(dataFileName, 91316020, 38, 15140868);
            // imageHelper.OutputImageInformation(image);

            // //Test: Read TMPLQ00.WMS (located at 91312748).  It's equivalent image is located at 14596100.
            // Console.WriteLine();
            // image = imageHelper.GetImage(dataFileName, 91312748, 23, 14596100);
            // imageHelper.OutputImageInformation(image);

            Console.ReadLine();
        }

    }
}
