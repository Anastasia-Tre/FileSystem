using System.Collections.Generic;

namespace FileSystem.Descriptors
{
    internal abstract class ObjectDescriptor
    {
        protected static int NextId = 1;
        public List<string> Links;

        protected int Nlink;

        protected ObjectDescriptor(string path)
        {
            Id = NextId++;
            Name = GetNameFromPath(path);
            Path = path;
            Links = new List<string> { path };
            Nlink++;
        }

        public ObjectDescriptors Type { get; protected set; }
        public int Id { get; }
        public string Name { get; private set; }
        public string Path { get; private set; }

        public abstract string Stat();
        public abstract string GetType();

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

        public string GetNameFromPath(string path)
        {
            return path.Substring(path.LastIndexOf('/') + 1);
        }

        public void Rename(string name)
        {
            Links.Remove(Path);
            Path = $"{Path.Remove(Path.IndexOf(Name))}{name}";
            Name = name;
            Links.Add(Path);
        }
    }
}
