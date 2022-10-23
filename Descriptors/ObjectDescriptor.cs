using System.Collections.Generic;

namespace FileSystem.Descriptors
{
    internal class ObjectDescriptor
    {
        protected static int NextId = 1;

        public List<string> Links;
        protected int Nblock = 0;
        protected int Size = 0;

        public ObjectDescriptor(string name, string path)
        {
            Id = NextId++;
            Name = name;
            Path = path;
            Links = new List<string> { path };
        }

        public ObjectDescriptors Type { get; protected set; }
        public int Id { get; }
        public string Name { get; }
        public string Path { get; }

        public string Stat()
        {
            return $"id={Id}, type={Type}, " +
                   $"nlink={Links.Count}, size={Size}, nblock={Nblock}";
        }

        public void AddLink(string path)
        {
            Links.Add(path);
        }

        public void RemoveLink(string path)
        {
            Links.Remove(path);
        }
    }
}
