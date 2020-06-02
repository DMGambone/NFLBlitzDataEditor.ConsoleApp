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
            string imagePath = Path.Combine(imageBasePath, imageInfo.ImageName);

            //Get the image data
            ImageDataReader reader = new ImageDataReader();
            ImageData imageData = null;
            using (Stream imageStream = File.OpenRead(imagePath))
            {
                imageData = reader.Read(new BinaryReader(imageStream));
            }

            int size = (int)(imageInfo.Width * imageInfo.Height);
            uint[] slicedImageData = new uint[size];
            int sourceX = (int)(imageInfo.X1 * imageData.Width);
            int sourceY = (int)(imageInfo.Y1 * imageData.Height);
            int sourceImageStart = (int)(sourceY * imageData.Width) + sourceX;
            for (int h = 0; h < (int)imageInfo.Height; h++)
            {
                int sourceIndex = sourceImageStart + (h * (int)imageData.Width);
                int destinationIndex = h * (int)imageInfo.Width;
                Array.Copy(imageData.Data, sourceIndex, slicedImageData, destinationIndex, (int)imageInfo.Width);
            }

            return new ImageData()
            {
                Version = imageData.Version,
                Bias = imageData.Bias,
                FilterMode = imageData.FilterMode,
                UseTrilinearFiltering = imageData.UseTrilinearFiltering,
                Width = (int)imageInfo.Width,
                Height = (int)imageInfo.Height,
                SmallestLOD = imageData.SmallestLOD,
                LargestLOD = imageData.LargestLOD,
                AspectRatio = imageData.AspectRatio,
                Format = imageData.Format,
                Data = slicedImageData
            };
        }
    }
}