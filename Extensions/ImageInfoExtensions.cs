using System;
using System.Linq;
using System.Text;
using NFLBlitzDataEditor.Core.Models;

namespace NFLBlitzDataEditor.ConsoleApp.Extensions
{
    public static class ImageInfoExtensions
    {
        public static string ConvertToString(this ImageInfo imageInfo, ImageData image = null)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("| {0,16} ", imageInfo.ImageName);

            stringBuilder.AppendFormat("| {0:000} ", imageInfo.Width);
            stringBuilder.AppendFormat("| {0:000} ", imageInfo.Height);
            stringBuilder.AppendFormat("| {0:000} ", imageInfo.AnchorX);
            stringBuilder.AppendFormat("| {0:000} ", imageInfo.AnchorY);

            stringBuilder.AppendFormat("| {0} ", imageInfo.X1);
            stringBuilder.AppendFormat("| {0} ", imageInfo.X2);
            stringBuilder.AppendFormat("| {0} ", imageInfo.Y1);
            stringBuilder.AppendFormat("| {0} ", imageInfo.Y2);
            if (image != null)
            {
                stringBuilder.AppendFormat("| {0} ", (int)(image.Width * imageInfo.X1));
                stringBuilder.AppendFormat("| {0} ", (int)(image.Width * imageInfo.X2));
                stringBuilder.AppendFormat("| {0} ", (int)(image.Height * imageInfo.Y1));
                stringBuilder.AppendFormat("| {0} ", (int)(image.Height * imageInfo.Y2));
            }

            return stringBuilder.ToString();
        }

        public static string ConvertImageDataHeaderToString(this ImageData imageData)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("| {0:X8} ", imageData.Version);
            stringBuilder.AppendFormat("| {0:X8} ", (int)imageData.Bias);
            stringBuilder.AppendFormat("| {0,14} ", imageData.FilterMode.ToString());
            stringBuilder.AppendFormat("| {0,5} ", imageData.UseTrilinearFiltering.ToString());
            stringBuilder.AppendFormat("| {0:000} ", imageData.Width);
            stringBuilder.AppendFormat("| {0:000} ", imageData.Height);
            stringBuilder.AppendFormat("| {0,6} ", imageData.SmallestLOD);
            stringBuilder.AppendFormat("| {0,6} ", imageData.LargestLOD);
            stringBuilder.AppendFormat("| {0,14} ", imageData.AspectRatio);
            stringBuilder.AppendFormat("| {0,16} ", imageData.Format);

            return stringBuilder.ToString();
        }

    }
}