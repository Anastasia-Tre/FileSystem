namespace FileSystem.Descriptors
{
    internal class ObjectDescriptor
    {
        protected static int NextId = 1;
        protected int Nblock = 0;

        protected int Nlink = 0;
        protected int Size = 0;

        public ObjectDescriptor(string name, string path)
        {
            Id = NextId++;
            Name = name;
            Path = path;
        }

        public ObjectDescriptors Type { get; protected set; }
        public int Id { get; }
        public string Name { get; }
        public string Path { get; }

        public string Stat()
        {
            return $"id={Id}, type={Type}, " +
                   $"nlink={Nlink}, size={Size}, nblock={Nblock}";
        }
    }
}
