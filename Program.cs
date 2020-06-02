using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using MidwayGamesFS;
using NFLBlitzDataEditor.Core;
using NFLBlitzDataEditor.Core.Readers;
using NFLBlitzDataEditor.Core.Models;
using NFLBlitzDataEditor.ConsoleApp.Extensions;
using NFLBlitzDataEditor.ConsoleApp.Helpers;


namespace NFLBlitzDataEditor.ConsoleApp
{
    class Program
    {
        static string _outputPath = "";
        static string _imagesPath = "";
        static string _soundsPath = "";

        static bool SaveAsImage(IFileSystem fileSystem, FileAllocationTableEntry entry, string outputPath)
        {
            ImageData image = fileSystem.ReadImage(entry.Name);
            if (image == null)
            {
                Console.WriteLine($"{entry.Name} (address: {entry.Position}-{entry.Position + entry.Size}) did not contain valid image data.");
                return false;
            }

            NFLBlitzImageHelper imageHelper = new NFLBlitzImageHelper();
            string path = Path.Combine(outputPath, $"{Path.GetFileNameWithoutExtension(entry.Name)}.png");
            imageHelper.SaveAsPNG(image, path);

            return true;
        }

        static void ExtractAllFiles(string dataFileName)
        {
            string fileManifestPath = Path.Combine(_outputPath, "file manifest.txt");
            if (File.Exists(fileManifestPath))
                File.Delete(fileManifestPath);

            IDataBuffer dataBuffer = new FileBuffer(dataFileName);
            IFileSystem fileSystem = new BlitzFileSystem(dataBuffer);
            IEnumerable<FileAllocationTableEntry> fileEntries = fileSystem.GetFiles();

            foreach (FileAllocationTableEntry entry in fileEntries)
            {
                File.AppendAllText(fileManifestPath, $"{entry.ConvertToString()}\n");
                string ext = Path.GetExtension(entry.Name);

                string path = _outputPath;
                if (string.Equals(ext, ".bnk", StringComparison.OrdinalIgnoreCase))
                    path = _soundsPath;
                else if (string.Equals(ext, ".wms", StringComparison.OrdinalIgnoreCase))
                {
                    path = _imagesPath;
                    SaveAsImage(fileSystem, entry, _imagesPath);
                }

                using (Stream fileStream = fileSystem.OpenRead(entry.Name))
                {
                    path = Path.Combine(path, $"{entry.Name}");
                    using (Stream outputStream = File.Open(path, FileMode.OpenOrCreate))
                        fileStream.CopyTo(outputStream);
                }
            }
        }

        static void Main(string[] args)
        {
            _outputPath = Path.Combine(Directory.GetCurrentDirectory(), "files");
            Directory.CreateDirectory(_outputPath);

            _imagesPath = Path.Combine(_outputPath, "images");
            Directory.CreateDirectory(_imagesPath);

            _soundsPath = Path.Combine(_outputPath, "sounds");
            Directory.CreateDirectory(_soundsPath);

            string dataFileName = @"C:\development\NFLBlitzDataEditor\Data Files\Blitz2kGold-arcade.bin";
            ExtractAllFiles(dataFileName);

            //Get the game file and extract the list of teams
            string gameFilePath = Path.Combine(_outputPath, "game.exe");
            using (Stream stream = File.OpenRead(gameFilePath))
            {
                IGameReader gameReader = new NFLBlitzDataEditor.Core.Readers.Blitz2kArcade.Blitz2kArcadeReader(stream);
                IEnumerable<Team> teams = gameReader.ReadAllTeams();

                foreach (Team team in teams)
                    Console.WriteLine(team.ConvertToString());
            }

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

    }
}
