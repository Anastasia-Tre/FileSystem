
namespace FileSystem.Descriptors
{
    internal class SymLinkDescriptor : ObjectDescriptor
    {
        public readonly ObjectDescriptor LinkedObject;

        public SymLinkDescriptor(string path, ObjectDescriptor obj) : base(path)
        {
            Type = ObjectDescriptors.Sym;
            LinkedObject = obj;
        }

        public override string Stat()
        {
            return $"id={Id}, type={Type}, nlink={Nlink}";
        }

        public override string GetType()
        {
            return $"{Type},{Id} -> {LinkedObject.Path}";
        }
    }
}
