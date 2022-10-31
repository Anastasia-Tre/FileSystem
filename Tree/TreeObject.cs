using System.Collections.Generic;
using FileSystem.Descriptors;

namespace FileSystem.Tree
{
    internal class TreeObject
    {
        public TreeObject Parent;
        public ObjectDescriptor Descriptor;
        public List<TreeObject> Children;

        public TreeObject(ObjectDescriptor descriptor, TreeObject parent)
        {
            Descriptor = descriptor;
            Parent = parent;
            Children = new List<TreeObject>();
        }

        public TreeObject AddChildren(TreeObject treeObject)
        {
            Children.Add(treeObject);
            return treeObject;
        }

        public void RemoveChildren(TreeObject treeObject)
        {
            Children.Remove(treeObject);
        }
    }
}
