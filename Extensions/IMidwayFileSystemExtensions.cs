using System;
using System.Linq;
using System.IO;
using NFLBlitzDataEditor.Core;
using NFLBlitzDataEditor.Core.FileSystem;
using NFLBlitzDataEditor.Core.Models;
using NFLBlitzDataEditor.Core.Readers;

namespace NFLBlitzDataEditor.ConsoleApp.Extensions
{
    public static class IMidwayFileSystemExtensions
    {
        public static ImageData ReadImage(this IMidwayFileSystem fileSystem, string fileName)
        {
            using (Stream stream = fileSystem.OpenRead(fileName))
            {
                BinaryReader reader = new BinaryReader(stream);
                ImageDataReader imageDataReader = new ImageDataReader();
                ImageData imageData = imageDataReader.Read(reader);

                return imageData;
            }
        }
    }
}