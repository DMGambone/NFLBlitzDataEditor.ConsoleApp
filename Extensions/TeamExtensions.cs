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
            stringBuilder.AppendFormat("{0} ", team.Name);
            stringBuilder.AppendFormat("| {0} ", team.TeamAbbreviation);
            stringBuilder.AppendFormat("| {0} ", String.Join(' ', team.UnknownRegion1.Select(b => b.ToString("x2")).ToArray()));
            stringBuilder.AppendFormat("| {0} ", team.CityName);
            stringBuilder.AppendFormat("| {0} ", team.CityAbbreviation);
            stringBuilder.AppendFormat("| {0:00} ", team.PassingRating);
            stringBuilder.AppendFormat("| {0:00} ", team.RushingRating);
            stringBuilder.AppendFormat("| {0:00} ", team.LinemenRating);
            stringBuilder.AppendFormat("| {0:00} ", team.DefenseRating);
            stringBuilder.AppendFormat("| {0:00} ", team.SpecialTeamsRating);
            stringBuilder.AppendFormat("| {0:00} ", team.PlayersOffset);
            stringBuilder.AppendFormat("| {0:00} ", team.FileOffset2);
            stringBuilder.AppendFormat("| {0:00} ", team.FileOffset3);
            stringBuilder.AppendFormat("| {0:00} ", team.FileOffset4);
            stringBuilder.AppendFormat("| {0:00} ", team.FileOffset5);
            stringBuilder.AppendFormat("| {0:00} ", team.FileOffset6);
            stringBuilder.AppendFormat("| {0:00} ", team.FileOffset7);

            return stringBuilder.ToString();
        }
    }
}