using System;
using System.Linq;
using System.Text;
using NFLBlitzDataEditor.Core.Models;

namespace NFLBlitzDataEditor.ConsoleApp.Extensions
{
    public static class PlayerExtensions
    {
        public static string ConvertToString(this Player player)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("| {0:00} ", (int)player.Size);
            stringBuilder.AppendFormat("| {0:00} ", (byte)player.SkinColor);
            stringBuilder.AppendFormat("| {0:00} ", player.Number);
            stringBuilder.AppendFormat("| {0:00} ", (byte)player.Position);
            stringBuilder.AppendFormat("| {0:00000000} ", player.UnknownValue2);
            stringBuilder.AppendFormat("| {0,-16} ", player.LastName);
            stringBuilder.AppendFormat("| {0,-16} ", player.FirstName);

            return stringBuilder.ToString();
        }
    }
}