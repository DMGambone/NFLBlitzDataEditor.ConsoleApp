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

            stringBuilder.AppendFormat("| {0:X} ", imageInfo.ImageAddress);

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

    }
}