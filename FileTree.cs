using System.Collections.Generic;
using System.Linq;
using FileSystem.Descriptors;

namespace FileSystem
{
    internal class FileTree
    {
        private TreeObject _current;
        private TreeObject _rootTreeObject;

        public FileTree(ObjectDescriptor rootDescriptor)
        {
            _rootTreeObject = new TreeObject(rootDescriptor, null);
            _current = _rootTreeObject;
        }

        public ObjectDescriptor GetObjectDescriptor(string path)
        {
            var names = path.Split('/');
            foreach (var name in names)
            {
                _current = _current.Children.First(obj => obj.Descriptor.Name == name);
            }
            return _current.Descriptor;
        }

    }

    internal class TreeObject
    {
        public TreeObject Parent;
        public ObjectDescriptor Descriptor;
        public List<TreeObject> Children;

        public TreeObject(ObjectDescriptor descriptor, TreeObject parent)
        {
            Descriptor = descriptor;
            Parent = parent;
            if (parent != null) Parent.AddChildren(this);
            Children = new List<TreeObject>();
        }

        public void AddChildren(TreeObject treeObject)
        {
            Children.Add(treeObject);
        }

        public void RemoveChildren(TreeObject treeObject)
        {
            Children.Remove(treeObject);
        }
    }
}
