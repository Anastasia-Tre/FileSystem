using System.Collections.Generic;

namespace FileSystem.Descriptors
{
    internal class FileDescriptor : ObjectDescriptor
    {
        private int _openFileHandlersNumber;
        private int Nblock = 0;
        private int Size = 0;

        public List<string> Links;

        public FileDescriptor(string name, string path) : base(name, path)
        {
            Type = ObjectDescriptors.File;
            Links = new List<string> { path };
            Nlink++;
        }

        public void Truncate(int size)
        {
            Size = size;
        }

        public void AddLink(string path)
        {
            Links.Add(path);
            Nlink++;
        }

        public void RemoveLink(string path)
        {
            Links.Remove(path);
            Nlink--;
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

        public override string Stat()
        {
            return $"id={Id}, type={Type}, " +
                   $"nlink={Nlink}, size={Size}, nblock={Nblock}";
        }

        public override string GetType()
        {
            return $"{Type},{Id}";
        }
    }
}
