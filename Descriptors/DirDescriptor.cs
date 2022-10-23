namespace FileSystem.Descriptors
{
    internal class DirDescriptor : ObjectDescriptor
    {
        private DirDescriptor _parentDir;

        public DirDescriptor(string name, string path, DirDescriptor parentDir) : base(name, path)
        {
            Type = ObjectDescriptors.DirDescriptor;
            _parentDir = parentDir;
            if (parentDir != null) parentDir.Nlink++;
            Nlink += 2;
        }
    }
}
