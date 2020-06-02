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
using MidwayGamesFS;

namespace NFLBlitzDataEditor.ConsoleApp.Helpers
{
    public class NFLBlitzImageHelper
    {
        private GameReader CreateDataFileReader(Stream stream)
        {
            return new NFLBlitzDataEditor.Core.Readers.Blitz2kArcade.Blitz2kArcadeReader(stream);
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

        public ImageData SliceImage(string imageBasePath, ImageInfo imageInfo)
        {
            ImageDataReader reader = new ImageDataReader();

            //Get the image data
            using (Stream imageStream = File.OpenRead(imageBasePath))
            {
                ImageData imageData = reader.Read(new BinaryReader(imageStream));
                
                return imageData;
            }

        }
    }
}