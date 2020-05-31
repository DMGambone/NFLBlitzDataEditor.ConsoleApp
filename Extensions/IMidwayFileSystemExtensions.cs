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

                CRC32 crc32 = new CRC32();
                stream.Seek(0, SeekOrigin.Begin);
                if(!crc32.ValidateCRC(stream))
                    Console.WriteLine($"{fileName} valid CRC validation.");

                return imageData;
            }
        }
    }
}