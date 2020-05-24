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

namespace NFLBlitzDataEditor.ConsoleApp.Helpers
{
    public class NFLBlitzImageHelper
    {
        private IDataFileReader CreateDataFileReader(Stream stream)
        {
            return new NFLBlitzDataEditor.Core.Readers.Blitz2kArcade.Blitz2kArcadeDataFileReader(stream);
        }

        private void SaveAsPNG(ImageData image, string destPath)
        {
            Rgba32[] pixels = image.Data.Select(pixel => new Rgba32(pixel)).ToArray();
            SixLabors.ImageSharp.Image outputImage = SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(pixels, image.Width, image.Height);

            using (System.IO.FileStream outputStream = System.IO.File.OpenWrite(destPath))
            {
                PngEncoder encoder = new PngEncoder();
                outputImage.Save(outputStream, encoder);
            }
        }

        private int FindInArray(int startIndex, byte[] find, byte[] buffer)
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

        public void ExtractAllImages(string dataFileName, string outputPath, bool groupByImageFormat, int startPosition = 0)
        {
            using (System.IO.Stream stream = System.IO.File.OpenRead(dataFileName))
            {
                byte[] findPattern = new byte[] {
                   0x05, 0x80, 0x00, 0x00
                    };

                uint readBatchSize = 1024 * 1024 * 1024;
                int batchStartPosition = startPosition;
                stream.Seek(batchStartPosition, SeekOrigin.Begin);
                IDictionary<int, ImageData> matches = new Dictionary<int, ImageData>();

                //Just focus on the inital 2GB of the data file
                byte[] buffer = new byte[readBatchSize];
                stream.Read(buffer, 0, (int)readBatchSize);

                IDataFileReader reader = CreateDataFileReader(stream);
                int offset = 0;
                while (offset < readBatchSize)
                {
                    int imageStart = FindInArray(offset, findPattern, buffer);
                    if (imageStart == -1)
                        break;

                    ImageData image = reader.ReadImageData(batchStartPosition + imageStart);
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
                    ImageData image = matches[key];

                    string path = outputPath;
                    if (groupByImageFormat)
                        path = Path.Combine(path, image.Format.ToString());
                    System.IO.Directory.CreateDirectory(path);
                    SaveAsPNG(image, System.IO.Path.Combine(path, $"{key.ToString("000000000")}.png"));
                }
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
                IDataFileReader reader = CreateDataFileReader(stream);
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