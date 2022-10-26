namespace FileSystem.Descriptors
{
    internal class FileDescriptor : ObjectDescriptor
    {
        private int _openFileHandlersNumber;
        private int Nblock = 0;
        private int Size = 0;
        public bool Created;

        public FileDescriptor(string path, bool created = true) : base(path)
        {
            Type = ObjectDescriptors.File;
            Created = created;
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
