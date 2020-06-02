using System;
using System.Linq;
using System.Text;
using NFLBlitzDataEditor.Core.Models;

namespace NFLBlitzDataEditor.ConsoleApp.Extensions
{
    public static class TeamExtensions
    {
        public static string ConvertToString(this Team team)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("| {0:00} ", team.PassingRating);
            stringBuilder.AppendFormat("| {0:00} ", team.RushingRating);
            stringBuilder.AppendFormat("| {0:00} ", team.LinemenRating);
            stringBuilder.AppendFormat("| {0:00} ", team.DefenseRating);
            stringBuilder.AppendFormat("| {0:00} ", team.SpecialTeamsRating);
            stringBuilder.AppendFormat("| {0:00} ", team.Reserved1);
            stringBuilder.AppendFormat("| {0:00} ", team.DroneBase);
            stringBuilder.AppendFormat("| {0,-14} ", team.Name);
            stringBuilder.AppendFormat("| {0,-3} ", team.TeamAbbreviation);
            stringBuilder.AppendFormat("| {0,-14} ", team.CityName);
            stringBuilder.AppendFormat("| {0,-3} ", team.CityAbbreviation);
            stringBuilder.AppendFormat("| 0x{0:x8} ", team.PlayersAddress);
            stringBuilder.AppendFormat("| 0x{0:x8} ", team.LogoAddress);
            stringBuilder.AppendFormat("| 0x{0:x8} ", team.Logo30Address);
            stringBuilder.AppendFormat("| 0x{0:x8} ", team.NameAddress);
            stringBuilder.AppendFormat("| 0x{0:x8} ", team.SelectedNameAddress);
            stringBuilder.AppendFormat("| 0x{0:x8} ", team.Reserved2);
            stringBuilder.AppendFormat("| 0x{0:x8} ", team.LoadingScreenImagesAddress);

            return stringBuilder.ToString();
        }
    }
}