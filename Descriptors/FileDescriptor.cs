using System.Collections.Generic;

namespace FileSystem.Descriptors
{
    internal class FileDescriptor : ObjectDescriptor
    {
        private int _openFileHandlersNumber;

        public FileDescriptor(string name, string path) : base(name, path)
        {
            Type = ObjectDescriptors.FileDescriptor;
        }

        public void Truncate(int size)
        {
            Size = size;
        }

        public int IncreaseNblock()
        {
            return Nblock++;
        }

        public int OpenFileHandler()
        {
            return _openFileHandlersNumber++;
        }

        public int CloseFileHandler()
        {
            return _openFileHandlersNumber--;
        }

        public bool CanBeRemoved()
        {
            return _openFileHandlersNumber == 0 && Links.Count == 0;
        }
    }
}
