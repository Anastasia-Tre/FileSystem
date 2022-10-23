using System;
using FileSystem.Descriptors;

namespace FileSystem
{
    internal class FileHandler
    {
        private const int MaxFileHandlersNumber = 10;

        private static readonly FileHandler[] FileHandlers =
            new FileHandler[MaxFileHandlersNumber];

        private static readonly DataHandler DataHandler = new();
        private readonly FileDescriptor _descriptor;
        private int _currentPosition;

        private readonly int _id = -1;

        public FileHandler(FileDescriptor descriptor)
        {
            for (var i = 0; i < MaxFileHandlersNumber; i++)
                if (FileHandlers[i] == null)
                {
                    FileHandlers[i] = this;
                    _id = i;
                    break;
                }

            if (_id == -1) throw new Exception();
            _descriptor = descriptor;
            _descriptor.OpenFileHandler();
        }

        public static void CreateFile(FileDescriptor descriptor)
        {
            DataHandler.Create(descriptor);
        }

        public static void RemoveFile(FileDescriptor descriptor)
        {
            DataHandler.Remove(descriptor);
        }

        public void CloseFile()
        {
            FileHandlers[_id]._descriptor.CloseFileHandler();
            if (FileHandlers[_id]._descriptor.CanBeRemoved())
                DataHandler.Remove(FileHandlers[_id]._descriptor);
            FileHandlers[_id] = null;
            Console.WriteLine($"The file with Id = {_descriptor.Id} was closed");
        }

        public void Seek(int offset)
        {
            _currentPosition = offset;
            Console.WriteLine(
                $"The file with Id = {_descriptor.Id} was seeked to {offset}");
        }

        public string Read(int size)
        {
            var result = DataHandler.Read(_descriptor, _currentPosition, size);
            Console.WriteLine($"The file with Id = {_descriptor.Id} was read: {result}");
            return result;
        }

        public void Write(int size, string str)
        {
            DataHandler.Write(_descriptor, str, _currentPosition, size);
            Console.WriteLine($"To the file with Id = {_descriptor.Id} was written: {str}");
        }
    }
}
