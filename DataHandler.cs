﻿using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FileSystem.Descriptors;

namespace FileSystem
{
    internal class DataHandler
    {
        private const string FileName = "data.txt";
        private const int BlockSize = 128;

        public Dictionary<string, List<string>> Data;

        public DataHandler()
        {
            Data = new Dictionary<string, List<string>>();
            File.WriteAllText(FileName, JsonSerializer.Serialize(Data));
        }

        public void Create(FileDescriptor descriptor)
        {
            if (!Data.ContainsKey(descriptor.Path))
                Data.Add(descriptor.Path, new List<string>());
            File.WriteAllText(FileName, JsonSerializer.Serialize(Data));
        }

        public void Remove(FileDescriptor descriptor)
        {
            Data.Remove(descriptor.Path);
            File.WriteAllText(FileName, JsonSerializer.Serialize(Data));
        }

        public void Write(FileDescriptor descriptor, string text, int offset,
            int size)
        {
            var blockIndex = offset / BlockSize;
            var blockOffset = offset % BlockSize;
            var count = Data[descriptor.Path].Count;

            if (count <= blockIndex)
                for (var i = 0; i <= blockIndex - count; i++)
                    Data[descriptor.Path].Add(null);

            if (Data[descriptor.Path][blockIndex] == null)
            {
                descriptor.IncreaseNblock();
                Data[descriptor.Path][blockIndex] =
                    new string(char.MinValue, BlockSize);
            }

            Data[descriptor.Path][blockIndex] =
                Data[descriptor.Path][blockIndex].Insert(blockOffset, text);
            File.WriteAllText(FileName, JsonSerializer.Serialize(Data));
        }

        public string Read(FileDescriptor descriptor, int offset, int size)
        {
            var blockIndex = offset / BlockSize;
            var blockOffset = offset % BlockSize;
            var blockNumber = size / BlockSize;

            var result = "";
            for (var i = blockIndex; i < blockNumber; i++)
                result += "[" + Data[descriptor.Path][i] + "]";

            if (size % BlockSize != 0)
                result += "[" + Data[descriptor.Path][blockIndex]
                    .Substring(blockOffset, size % BlockSize) + "]";
            return result;
        }
    }
}
