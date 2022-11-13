using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FileSystem.Descriptors;

namespace FileSystem
{
    internal class DataHandler
    {
        private const string FileName = "data.txt";
        private const int BlockSize = 128;

        public Dictionary<int, List<string>> Data;

        public DataHandler()
        {
            Data = new Dictionary<int, List<string>>();
            File.WriteAllText(FileName, JsonSerializer.Serialize(Data));
        }

        public void Create(FileDescriptor descriptor)
        {
            if (!Data.ContainsKey(descriptor.Id))
                Data.Add(descriptor.Id, new List<string>());
            File.WriteAllText(FileName, JsonSerializer.Serialize(Data));
        }

        public void Remove(FileDescriptor descriptor)
        {
            Data.Remove(descriptor.Id);
            File.WriteAllText(FileName, JsonSerializer.Serialize(Data));
        }

        public void Write(FileDescriptor descriptor, string text, int offset,
            int size)
        {
            var id = descriptor.Id;
            var blockIndex = offset / BlockSize;
            var blockOffset = offset % BlockSize;
            var blockNumber = size / BlockSize;
            var count = Data[id].Count;

            if (count <= blockIndex)
                for (var i = 0; i <= blockIndex - count + blockNumber; i++)
                    Data[id].Add(null);

            if (Data[id][blockIndex] == null)
            {
                descriptor.IncreaseNblock();
                Data[id][blockIndex] = new string(char.MinValue, BlockSize);
            }

            for (var i = 0; i < blockNumber; i++)
            {
                if (Data[id][blockIndex + i] == null)
                    descriptor.IncreaseNblock();
                Data[id][blockIndex + i] =
                    text.Substring(BlockSize * i, BlockSize);
            }

            if (Data[id][blockIndex + blockNumber] == null)
            {
                descriptor.IncreaseNblock();
                Data[id][blockIndex + blockNumber] =
                    new string(char.MinValue, BlockSize);
            }

            Data[id][blockIndex + blockNumber] =
                Data[id][blockIndex + blockNumber]
                    .Remove(blockOffset, size % BlockSize);
            Data[id][blockIndex + blockNumber] =
                Data[id][blockIndex + blockNumber].Insert(
                    blockOffset,
                    text.Substring(BlockSize * blockNumber, size % BlockSize));

            File.WriteAllText(FileName, JsonSerializer.Serialize(Data));
        }

        public string Read(FileDescriptor descriptor, int offset, int size)
        {
            var blockIndex = offset / BlockSize;
            var blockOffset = offset % BlockSize;
            var blockNumber = size / BlockSize;

            var result = "";
            for (var i = blockIndex; i < blockNumber; i++)
                result += "[" + Data[descriptor.Id][i] + "]";

            if (size % BlockSize != 0)
                result += "[" + Data[descriptor.Id][blockIndex + blockNumber]
                    .Substring(blockOffset, size % BlockSize) + "]";
            return result;
        }
    }
}
