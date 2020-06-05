using System;
using System.Linq;
using System.Text;
using NFLBlitzDataEditor.Core.Models;

namespace NFLBlitzDataEditor.ConsoleApp.Extensions
{
    public static class PlaybookExtensions
    {
        public static string ConvertToString(this PlaybookEntry entry)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendFormat("| {0,16} ", entry.Name);
            strBuilder.AppendFormat("| {0:x8} ", entry.PlayDataAddress);
            strBuilder.AppendFormat("| {0:000} ", entry.Unknown1);
            strBuilder.AppendFormat("| {0:x8} ", entry.Unknown2);
            strBuilder.AppendFormat("| {0:x8} ", entry.Unknown3);

            return strBuilder.ToString();
        }

        public static string ConvertToString(this PlayStartingPosition startingPosition)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendFormat("| {0}{1:000} ", (startingPosition.X >= 0 ? " " : ""), startingPosition.X);
            strBuilder.AppendFormat("| {0}{1:000} ", (startingPosition.Z >= 0 ? " " : ""), startingPosition.Z);
            strBuilder.AppendFormat("| {0:x8} ", startingPosition.PreSnapActionsAddress);
            strBuilder.AppendFormat("| {0:x8} ", startingPosition.Role);
            strBuilder.AppendFormat("| {0:x8} ", startingPosition.ControlFlag);

            return strBuilder.ToString();
        }

        public static string ConvertToString(this PlayRouteAction routeAction)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendFormat("| {0}{1:000} ", (routeAction.X >= 0 ? " " : ""), routeAction.X);
            strBuilder.AppendFormat("| {0}{1:000} ", (routeAction.Z >= 0 ? " " : ""), routeAction.Z);
            strBuilder.AppendFormat("| {0:x8}", routeAction.Unknown);
            strBuilder.AppendFormat("| {0}", routeAction.Action.ToString());

            return strBuilder.ToString();
        }

    }
}