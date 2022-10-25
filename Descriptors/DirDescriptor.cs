using System;

namespace FileSystem.Descriptors
{
    internal class DirDescriptor : ObjectDescriptor
    {
        public DirDescriptor ParentDir;

        public DirDescriptor(string path, DirDescriptor parentDir) : base(path)
        {
            Type = ObjectDescriptors.Dir;
            ParentDir = parentDir;
            if (parentDir != null) parentDir.Nlink++;
            else ParentDir = this;
            Nlink++;
        }

        public override string Stat()
        {
            return $"id={Id}, type={Type}, nlink={Nlink}";
        }

        public override string GetType()
        {
            return $"{Type},{Id}";
        }
    }
}
