namespace FileSystem.Descriptors
{
    internal class DirDescriptor : ObjectDescriptor
    {
        public DirDescriptor(string name, string path) : base(name, path)
        {
            Type = ObjectDescriptors.DirDescriptor;
        }
    }
}
