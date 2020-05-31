using System;
using System.Linq;
using System.Text;
using NFLBlitzDataEditor.Core.FileSystem;

namespace NFLBlitzDataEditor.ConsoleApp.Extensions
{
    public static class FileSystemEntryExtensions
    {
        public static string ConvertToString(this FileAllocationTableEntry entry)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0, 12} ", entry.Name);
            stringBuilder.AppendFormat("| {0:0000000} ", entry.Size);
            stringBuilder.AppendFormat("| {0:u} ", entry.Timestamp);
            stringBuilder.AppendFormat("| {0} ", entry.Address);

            return stringBuilder.ToString();
        }
    }
}