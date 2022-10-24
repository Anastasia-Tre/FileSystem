using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem.Descriptors
{
    internal class SymLinkDescriptor : ObjectDescriptor
    {
        private readonly ObjectDescriptor _linkedObject;

        public SymLinkDescriptor(string name, string path, ObjectDescriptor obj) : base(name, path)
        {
            Type = ObjectDescriptors.Sym;
            _linkedObject = obj;
        }

        public override string Stat()
        {
            return $"id={Id}, type={Type}, nlink={Nlink}";
        }

        public override string GetType()
        {
            return $"{Type},{Id} -> {_linkedObject.Name}";
        }
    }
}
