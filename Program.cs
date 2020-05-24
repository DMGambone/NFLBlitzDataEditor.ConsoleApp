using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using NFLBlitzDataEditor.Core.Readers;
using NFLBlitzDataEditor.Core.Models;
using NFLBlitzDataEditor.Core.Enums;
using NFLBlitzDataEditor.ConsoleApp.Extensions;
using Newtonsoft.Json;

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

        static void ExtractImages(string dataFileName)
        {
            using (System.IO.Stream stream = System.IO.File.OpenRead(dataFileName))
            {
                byte[] findPattern = new byte[] {
                   0x05, 0x80, 0x00, 0x00
                    };

                uint readBatchSize = 1024 * 1024 * 1024;
                int batchStartPosition = 0;
                stream.Seek(batchStartPosition, SeekOrigin.Begin);
                IDictionary<int, Image> matches = new Dictionary<int, Image>();

                //Just focus on the inital 2GB of the data file
                byte[] buffer = new byte[readBatchSize];
                stream.Read(buffer, 0, (int)readBatchSize);

                IDataFileReader reader = new NFLBlitzDataEditor.Core.Readers.Blitz2kArcade.Blitz2kArcadeDataFileReader(stream);
                int offset = 0;
                while (offset < readBatchSize)
                {
                    int imageStart = FindInArray(offset, findPattern, buffer);
                    if (imageStart == -1)
                        break;

                    Image image = reader.ReadImage(batchStartPosition + imageStart);
                    if (image == null)
                    {
                        //False match, advance to immediately after the false match
                        offset = imageStart + 1;
                        continue;
                    }

                    matches.Add(imageStart, image);
                    int dataLength = 40 + (image.Data.Length * (image.Format == ImageFormat.BitmapMask ? 1 : 2));
                    offset = imageStart + dataLength;
                }

                foreach (int key in matches.Keys.OrderBy(key => key).ToArray())
                {
                    Image image = matches[key];

                    Console.WriteLine("Image found:\t{0}\t{1}\t{2}", key, image.Format, image.Data.Length);
                    string path = System.IO.Path.Combine(".\\images", image.Format.ToString());
                    System.IO.Directory.CreateDirectory(path);
                    image.SaveAsPNG(System.IO.Path.Combine(path, $"{key.ToString("000000000")}.png"));
                }
            }
        }

        static void Main(string[] args)
        {
            string dataFileName = @"c:\mame\roms\blitz2k\blitz2k-arcade.bin";

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

<<<<<<< Updated upstream
            //System.IO.Directory.CreateDirectory("images");
            //ExtractImages(dataFileName);


            //Test: Read TMSEL00.WMS (located at 91313848).  It's equivalent image is located at 15005700.
            Console.WriteLine();
            using (Stream stream = File.OpenRead(dataFileName))
            {
                IDataFileReader reader = new NFLBlitzDataEditor.Core.Readers.Blitz2kArcade.Blitz2kArcadeDataFileReader(stream);
                ImageTable imageTable = reader.ReadImageTable(91313848, 60);
                Image image = reader.ReadImage(15005700);

                Console.WriteLine($"Image table for {imageTable.Name}");
                foreach (ImageInfo imageInfo in imageTable.Entries)
                    Console.WriteLine(imageInfo.ConvertToString(image));
            }


=======
            System.IO.Directory.CreateDirectory("images");
            //ExtractImages(dataFileName);

>>>>>>> Stashed changes
            Console.ReadLine();
        }

    }
}
