using System.Collections.Generic;
using System.Linq;
using FileSystem.Descriptors;

namespace FileSystem.FileTree
{
    internal class FileTree
    {
        public static int ObjectNumber;
        public TreeObject CWD;
        private readonly TreeObject _rootTreeObject;

        public FileTree(ObjectDescriptor rootDescriptor)
        {
            _rootTreeObject = new TreeObject(rootDescriptor, null);
            _rootTreeObject.Parent = _rootTreeObject;
            CWD = _rootTreeObject;
        }

        public ObjectDescriptor GetObjectDescriptor(string name)
        {
            var path = GetPath(name);
            return GetTreeObject(path)?.Descriptor;
        }

        public string GetPath(string name)
        {
            if (name.StartsWith('/')) return name;
            return CWD.Descriptor.Path == "/" ? $"/{name}" : $"{CWD.Descriptor.Path}/{name}";
        }

        private TreeObject GetTreeObject(string path)
        {
            if (path == "/") return _rootTreeObject;
            var names = path.Split('/');
            var temp = _rootTreeObject;
            for (var i = 1; i < names.Length; i++)
            {
                temp = temp?.Children.FirstOrDefault(obj => {
                    if (obj.Descriptor is FileDescriptor fileDescriptor)
                    {
                        return fileDescriptor.Links.Contains(path);
                    }
                    return obj.Descriptor.Name == names[i];
                });
            }
            return temp;
        }

        public void Cd(string name)
        {
            var path = GetPath(name);
            CWD = GetTreeObject(path);
        }

        public List<ObjectDescriptor> Ls(string path)
        {
            var startObject = GetTreeObject(path);
            return startObject.Children.Select(treeObject => treeObject.Descriptor).ToList();
        }

        public TreeObject AddTreeObject(ObjectDescriptor descriptor)
        {
            return CWD.AddChildren(new TreeObject(descriptor, CWD));
        }

        public void RemoveTreeObject(string path)
        {
            var treeObject = GetTreeObject(path);
            treeObject.Parent.RemoveChildren(treeObject);
        }
    }
}
