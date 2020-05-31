using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using NFLBlitzDataEditor.Core.Readers;
using NFLBlitzDataEditor.Core.Models;
using NFLBlitzDataEditor.Core.Enums;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;
using NFLBlitzDataEditor.ConsoleApp.Extensions;
using NFLBlitzDataEditor.Core.FileSystem;

namespace NFLBlitzDataEditor.ConsoleApp.Helpers
{
    public class NFLBlitzImageHelper
    {
        private DataFileReader CreateDataFileReader(Stream stream)
        {
            return new NFLBlitzDataEditor.Core.Readers.Blitz2kArcade.Blitz2kArcadeDataFileReader(stream);
        }

        public void SaveAsPNG(ImageData image, string destPath)
        {
            Rgba32[] pixels = image.Data.Select(pixel => new Rgba32(pixel)).ToArray();
            SixLabors.ImageSharp.Image outputImage = SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(pixels, image.Width, image.Height);

            using (System.IO.FileStream outputStream = System.IO.File.OpenWrite(destPath))
            {
                PngEncoder encoder = new PngEncoder();
                outputImage.Save(outputStream, encoder);
            }
        }

        /// <summary>
        /// Reads image information from the data file
        /// </summary>
        /// <param name="dataFileName">The data file to read the image information from</param>
        /// <param name="imageInfoAddress">The starting location of the image info record</param>
        /// <param name="numberOfEntries">The number of entries in the image</param>
        /// <param name="imageAddress">The address of the image</param>
        public Image GetImage(string dataFileName, uint imageTableAddress, uint numberOfEntries, uint imageAddress)
        {
            using (Stream stream = File.OpenRead(dataFileName))
            {
                DataFileReader reader = CreateDataFileReader(stream);
                return reader.ReadImage(imageTableAddress, numberOfEntries, imageAddress);
            }
        }

        /// <summary>
        /// Outputs information about an NFL Blitz image to the console
        /// </summary>
        /// <param name="image">An instance of <see cref="Image" /> to output information on</param>
        public void OutputImageInformation(Image image)
        {
            Console.WriteLine($"Image information for {image.Info.Name}");
            foreach (ImageInfo imageInfo in image.Info.Entries)
                Console.WriteLine(imageInfo.ConvertToString(image.Data));
        }

    }
}