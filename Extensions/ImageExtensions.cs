using System;
using System.Linq;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;

namespace NFLBlitzDataEditor.ConsoleApp.Extensions
{
    public static class ImageExtensions
    {
        public static void SaveAsPNG(this NFLBlitzDataEditor.Core.Models.Image image, string destPath)
        {
            Rgba32[] pixels = image.Data.Select(pixel => new Rgba32(pixel)).ToArray();
            Image outputImage = Image.LoadPixelData<Rgba32>(pixels, image.Width, image.Height);

            using (System.IO.FileStream outputStream = System.IO.File.OpenWrite(destPath))
            {
                PngEncoder encoder = new PngEncoder();
                outputImage.Save(outputStream, encoder);
            }
        }
    }
}