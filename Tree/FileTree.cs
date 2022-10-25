using System;
using System.Collections.Generic;
using System.Linq;
using FileSystem.Descriptors;

namespace FileSystem.Tree
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
                if (names[i] == ".")
                {
                    continue;
                }
                if (names[i] == "..")
                {
                    temp = temp.Parent;
                    continue;
                }
                if (names[i] == "") continue;
                temp = temp?.Children.FirstOrDefault(obj => {
                    if (obj.Descriptor is FileDescriptor fileDescriptor)
                    {
                        foreach (var link in fileDescriptor.Links)
                        {
                            return fileDescriptor.GetNameFromPath(link) ==
                                   names[i];
                        }
                    }
                    return obj.Descriptor.Name == names[i];
                });
            }
            return temp;
        }

        public void Cd(string name)
        {
            var treeObject = GetTreeObject(GetPath(name));
            if (treeObject == null) return;
            CWD = GetTreeObject(GetPath(name));
        }

        public List<ObjectDescriptor> Ls(string path)
        {
            var startObject = GetTreeObject(path);
            return startObject.Children.Select(treeObject => treeObject.Descriptor).ToList();
        }

        public TreeObject AddTreeObject(ObjectDescriptor descriptor)
        {
            var path = descriptor.Path.Remove(descriptor.Path.LastIndexOf('/'));
            var parent = GetTreeObject(path);
            if (descriptor is SymLinkDescriptor symLinkDescriptor)
            {
                var linkedTreeObject = GetTreeObject(symLinkDescriptor.LinkedObject.Path);
                var treeObject = new TreeObject(symLinkDescriptor,
                    linkedTreeObject.Parent)
                {
                    Children = linkedTreeObject.Children
                };
                return parent.AddChildren(treeObject);
            }
            return parent.AddChildren(new TreeObject(descriptor, parent));
        }

        public void RemoveTreeObject(string path)
        {
            var treeObject = GetTreeObject(path);
            treeObject.Parent.RemoveChildren(treeObject);
        }
    }
}
