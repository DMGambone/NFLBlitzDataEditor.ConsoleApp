using System;
using System.IO;
using System.Collections.Generic;
using MidwayGamesFS;

namespace NFLBlitzDataEditor.ConsoleApp.Helpers
{
    public class FileBuffer
        : IDataBuffer
    {
        private string _fileName;

        public FileBuffer(string fileName)
        {
            _fileName = fileName;
        }

        /// <inheritdocs>
        public BinaryReader GetReader(int position, int size)
        {
            return new BinaryReader(new MemoryStream(Get(position, size)));
        }

        /// <inheritdocs>
        public byte[] Get(int position, int size)
        {
            using (Stream stream = File.OpenRead(_fileName))
            {
                stream.Seek(position, SeekOrigin.Begin);

                byte[] buffer = new byte[size];
                stream.Read(buffer);

                return buffer;
            }
        }

        /// <inheritdocs>
        public void Set(int position, byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}