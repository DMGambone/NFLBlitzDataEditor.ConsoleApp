using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using MidwayGamesFS;
using NFLBlitzDataEditor.Core;
using NFLBlitzDataEditor.Core.Extensions;
using NFLBlitzDataEditor.Core.Readers;
using NFLBlitzDataEditor.Core.Models;
using NFLBlitzDataEditor.ConsoleApp.Extensions;
using NFLBlitzDataEditor.ConsoleApp.Helpers;


namespace NFLBlitzDataEditor.ConsoleApp
{
    class Program
    {
        static string _outputPath = "";
        static string _imagesPath = "images";
        static string _soundsPath = "sounds";
        static string _teamsPath = "teams";

        static bool SaveAsImage(IFileSystem fileSystem, FileAllocationTableEntry entry, string outputPath)
        {
            try
            {
                ImageData image = fileSystem.ReadImage(entry.Name);

                NFLBlitzImageHelper imageHelper = new NFLBlitzImageHelper();
                string path = Path.Combine(outputPath, $"{Path.GetFileNameWithoutExtension(entry.Name)}.png");
                imageHelper.SaveAsPNG(image, path);

                return true;
            }
            catch (InvalidDataException)
            {
                Console.WriteLine($"{entry.Name} (address: {entry.Position}-{entry.Position + entry.Size}) did not contain valid image data.");
                return false;
            }
            catch
            {
                throw;
            }

        }

        static void ExtractAllFiles(IFileSystem fileSystem)
        {
            string fileManifestPath = Path.Combine(_outputPath, "file manifest.txt");
            if (File.Exists(fileManifestPath))
                File.Delete(fileManifestPath);

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
                    if (entry.Timestamp != DateTime.MinValue)
                    {
                        File.SetCreationTime(path, entry.Timestamp);
                        File.SetLastWriteTime(path, entry.Timestamp);
                    }
                }
            }
        }

        static void ExtractImage(ImageInfo imageInfo, string destPath)
        {
            NFLBlitzImageHelper imageHelper = new NFLBlitzImageHelper();
            ImageData imageData = imageHelper.SliceImage(_imagesPath, imageInfo);

            imageHelper.SaveAsPNG(imageData, destPath);
        }

        static void ExtractTeamImages(Team team, string outputPath)
        {
            string basePath = Path.Combine(outputPath, team.Name);
            Directory.CreateDirectory(basePath);

            ExtractImage(team.LogoImage, Path.Combine(basePath, "logo.png"));
            ExtractImage(team.Logo30Image, Path.Combine(basePath, "logo30.png"));
            ExtractImage(team.NameImage, Path.Combine(basePath, "name.png"));
            ExtractImage(team.SelectedNameImage, Path.Combine(basePath, "selected-name.png"));

            //Get the loading screen images
            int imageIndex = 0;
            foreach (ImageInfo loadingScreenImage in team.LoadingScreenBannerImages)
            {
                imageIndex++;
                ExtractImage(loadingScreenImage, Path.Combine(basePath, $"loading-screen-banner{imageIndex}.png"));
            }

            //Get the loading screen images
            imageIndex = 0;
            foreach (ImageInfo loadingScreenImage in team.LoadingScreenTeamNameImages)
            {
                imageIndex++;
                ExtractImage(loadingScreenImage, Path.Combine(basePath, $"loading-screen-teamname{imageIndex}.png"));
            }

            //Loop through all the players and extract the player images
            foreach (Player player in team.Players)
            {
                if (player.MugShotImage != null)
                    ExtractImage(player.MugShotImage, Path.Combine(basePath, $"{player.FirstName} {player.LastName} - mugshot.png"));

                if (player.NameImage != null)
                    ExtractImage(player.NameImage, Path.Combine(basePath, $"{player.FirstName} {player.LastName} - name.png"));

                if (player.SelectedNameImage != null)
                    ExtractImage(player.SelectedNameImage, Path.Combine(basePath, $"{player.FirstName} {player.LastName} - selected-name.png"));
            }
        }

        private static void OutputPlaybookEntry(PlaybookEntry entry, bool outputPlayDetails = false)
        {
            Console.WriteLine(entry.ConvertToString());
            if (outputPlayDetails &&
                entry.PlayData != null)
            {
                PlayStartingPosition[] formation = entry.PlayData.Formation.ToArray();
                PlayRoute[] routes = entry.PlayData.Routes.ToArray();
                for (int i = 0; i < 7; i++)
                {
                    Console.WriteLine("  {0,16} {1}", "", formation[i].ConvertToString());
                    foreach (PlayRouteAction routeAction in routes[i].Actions)
                    {
                        Console.WriteLine("  {0,30} {1}", "", routeAction.ConvertToString());
                    }
                }
            }
        }


        private static void OutputPlaybookEntries(IEnumerable<PlaybookEntry> playbookEntries, bool outputPlayDetails = false)
        {
            foreach (PlaybookEntry entry in playbookEntries)
                OutputPlaybookEntry(entry, outputPlayDetails);
        }

        static void Main(string[] args)
        {
            //string dataFileName = @"C:\development\NFLBlitzDataEditor\Data Files\Blitz2kGold-arcade.bin";
            string dataFileName = @"C:\development\NFLBlitzDataEditor\Data Files\Blitz2kGold-Mame with custom play.bin";

            _outputPath = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileNameWithoutExtension(dataFileName));
            Directory.CreateDirectory(_outputPath);

            _imagesPath = Path.Combine(_outputPath, _imagesPath);
            Directory.CreateDirectory(_imagesPath);

            _soundsPath = Path.Combine(_outputPath, _soundsPath);
            Directory.CreateDirectory(_soundsPath);

            _teamsPath = Path.Combine(_outputPath, _teamsPath);
            Directory.CreateDirectory(_teamsPath);

            IDataBuffer dataBuffer = new FileBuffer(dataFileName);
            IFileSystem fileSystem = new BlitzFileSystem(dataBuffer);
            ExtractAllFiles(fileSystem);

            //Get the game file and extract the list of teams
            string gameFilePath = Path.Combine(_outputPath, "game.exe");
            using (Stream stream = File.OpenRead(gameFilePath))
            {
                IGameReader gameReader = new Blitz2kArcadeReader(stream);
                IEnumerable<Team> teams = gameReader.ReadAllTeams();

                // foreach (Team team in teams)
                // {
                //     ExtractTeamImages(team, _teamsPath);
                // }

                Playbook playbook = gameReader.GetPlaybook();
                Console.WriteLine("Offsense Plays:");
                OutputPlaybookEntries(playbook.Offense);

                Console.WriteLine("Custom Plays:");
                OutputPlaybookEntries(playbook.Custom);

                Console.WriteLine("Defense Plays:");
                OutputPlaybookEntries(playbook.Defense);

                IEnumerator<IEnumerable<PlaybookEntry>> teamPlayEnumerator = playbook.TeamPlays.GetEnumerator();
                teamPlayEnumerator.Reset();
                foreach (Team team in teams)
                {
                    Console.WriteLine($"{team.Name}:");
                    teamPlayEnumerator.MoveNext();
                    OutputPlaybookEntries(teamPlayEnumerator.Current);
                }

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                OutputPlaybookEntry(playbook.Offense.FirstOrDefault(p => p.Name == "UP THE GUT"), true);

            }

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

    }
}
