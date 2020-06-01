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
            stringBuilder.AppendFormat("| {0} ", team.Reserved1);
            stringBuilder.AppendFormat("| {0} ", team.DroneBase);
            stringBuilder.AppendFormat("| {0} ", team.CityName);
            stringBuilder.AppendFormat("| {0} ", team.CityAbbreviation);
            stringBuilder.AppendFormat("| {0:00} ", team.PassingRating);
            stringBuilder.AppendFormat("| {0:00} ", team.RushingRating);
            stringBuilder.AppendFormat("| {0:00} ", team.LinemenRating);
            stringBuilder.AppendFormat("| {0:00} ", team.DefenseRating);
            stringBuilder.AppendFormat("| {0:00} ", team.SpecialTeamsRating);
            stringBuilder.AppendFormat("| {0:00} ", team.PlayersAddress);
            stringBuilder.AppendFormat("| {0:00} ", team.TeamLogoAddress);
            stringBuilder.AppendFormat("| {0:00} ", team.TeamLogo30Address);
            stringBuilder.AppendFormat("| {0:00} ", team.TeamSelectedNameAddress);
            stringBuilder.AppendFormat("| {0:00} ", team.TeamNameAddress);
            stringBuilder.AppendFormat("| {0:00} ", team.Reserved2);
            stringBuilder.AppendFormat("| {0:00} ", team.UnknownAddress);

            return stringBuilder.ToString();
        }
    }
}