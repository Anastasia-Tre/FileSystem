using System.Collections.Generic;

namespace FileSystem.Descriptors
{
    internal abstract class ObjectDescriptor
    {
        protected static int NextId = 1;

        protected int Nlink;

        public ObjectDescriptor(string path)
        {
            Id = NextId++;
            Name = GetNameFromPath(path);
            Path = path;
        }

        public ObjectDescriptors Type { get; protected set; }
        public int Id { get; }
        public string Name { get; }
        public string Path { get; }

        public abstract string Stat();
        public abstract string GetType();

        public string GetNameFromPath(string path)
        {
            return path.Substring(path.LastIndexOf('/') + 1);
        }
    }
}
