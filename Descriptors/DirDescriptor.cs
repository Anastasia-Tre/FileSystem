namespace FileSystem.Descriptors
{
    internal class DirDescriptor : ObjectDescriptor
    {
        public DirDescriptor ParentDir;

        public DirDescriptor(string name, string path, DirDescriptor parentDir) : base(name, path)
        {
            Type = ObjectDescriptors.DirDescriptor;
            ParentDir = parentDir;
            if (parentDir != null) parentDir.Nlink++;
            else ParentDir = this;
            Nlink += 2;
        }
    }
}
