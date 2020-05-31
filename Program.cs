using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using NFLBlitzDataEditor.Core.FileSystem;
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

        static bool SaveAsImage(IMidwayFileSystem fileSystem, FileAllocationTableEntry entry, string outputPath)
        {
            ImageData image = fileSystem.ReadImage(entry.Name);
            if (image == null)
            {
                Console.WriteLine($"{entry.Name} (address: {entry.Address}-{entry.Address + entry.Size}) did not contain valid image data.");
                return false;
            }

            NFLBlitzImageHelper imageHelper = new NFLBlitzImageHelper();
            string path = Path.Combine(outputPath, $"{Path.GetFileNameWithoutExtension(entry.Name)}.png");
            imageHelper.SaveAsPNG(image, path);

            return true;
        }

        static void Main(string[] args)
        {
            string dataFileName = @"C:\development\NFLBlitzDataEditor\Data Files\Blitz2kGold-arcade.bin";

            string outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "files");
            Directory.CreateDirectory(outputDirectory);

            string imagesDirectory = Path.Combine(outputDirectory, "images");
            Directory.CreateDirectory(imagesDirectory);

            string fileManifestPath = Path.Combine(outputDirectory, "file manifest.txt");
            if (File.Exists(fileManifestPath))
                File.Delete(fileManifestPath);

            using (Stream stream = File.Open(dataFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                IMidwayFileSystem fileSystem = new BlitzFileSystem(stream);
                IEnumerable<FileAllocationTableEntry> fileEntries = fileSystem.GetFiles();

                foreach (FileAllocationTableEntry entry in fileEntries)
                {
                    File.AppendAllText(fileManifestPath, $"{entry.ConvertToString()}\n");
                    if (entry.Name.EndsWith(".wms", StringComparison.OrdinalIgnoreCase)
                        && SaveAsImage(fileSystem, entry, imagesDirectory))
                        continue;

                    using (Stream fileStream = fileSystem.OpenRead(entry.Name))
                    {
                        string path = Path.Combine(outputDirectory, $"{entry.Name}");
                        using (Stream outputStream = File.Open(path, FileMode.OpenOrCreate))
                            fileStream.CopyTo(outputStream);
                    }
                }
            }

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

    }
}
