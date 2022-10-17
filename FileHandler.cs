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

        public FileHandler(FileDescriptor descriptor)
        {
            for (var i = 0; i < MaxFileHandlersNumber; i++)
                if (FileHandlers[i] == null)
                {
                    FileHandlers[i] = this;
                    Id = i;
                    break;
                }

            if (Id == -1) throw new Exception();
            _descriptor = descriptor;
            _descriptor.OpenFileHandler();
        }

        public int Id { get; } = -1;

        public static FileHandler GetFileHandlerById(int id)
        {
            try
            {
                return FileHandlers[id];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void CreateFile(FileDescriptor descriptor)
        {
            DataHandler.Create(descriptor);
        }

        public static void RemoveFile(FileDescriptor descriptor)
        {
            DataHandler.Remove(descriptor);
        }

        public void CloseFile(int fd)
        {
            FileHandlers[fd]._descriptor.CloseFileHandler();
            if (FileHandlers[fd]._descriptor.CanBeRemoved())
                DataHandler.Remove(FileHandlers[fd]._descriptor);
            FileHandlers[fd] = null;
        }

        public void Seek(int offset)
        {
            _currentPosition = offset;
        }

        public string Read(int size)
        {
            var result = DataHandler.Read(_descriptor, _currentPosition, size);
            return result;
        }

        public void Write(int size, string str)
        {
            DataHandler.Write(_descriptor, str, _currentPosition, size);
        }
    }
}
