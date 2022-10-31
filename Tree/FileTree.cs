﻿using System;
using System.Collections.Generic;
using System.Linq;
using FileSystem.Descriptors;

namespace FileSystem.Tree
{
    internal class FileTree
    {
        private static int ObjectNumber;
        private readonly int _maxDescriptorsNumber;
        private readonly int _maxFileNameLength = 128;

        public TreeObject CWD;
        private readonly TreeObject _rootTreeObject;

        public FileTree(ObjectDescriptor rootDescriptor, int maxDescrNumber)
        {
            _maxDescriptorsNumber = maxDescrNumber;
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
            var names = path.Split('/');
            var result = _rootTreeObject;
            for (var i = 1; i < names.Length; i++)
            {
                if (names[i] == "." || names[i] == "") continue;
                if (names[i] == "..")
                {
                    result = result?.Parent;
                    continue;
                }
                result = result?.Children.FirstOrDefault(obj =>
                    obj.Descriptor.Links.Exists(link =>
                        obj.Descriptor.GetNameFromPath(link) == names[i]));
            }
            return result;
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

        public TreeObject AddTreeObject(ObjectDescriptor descriptor, string objectPath = null)
        {
            var path = objectPath == null
                ? descriptor.Path.Remove(descriptor.Path.LastIndexOf('/'))
                : objectPath.Remove(objectPath.LastIndexOf('/'));
            var parent = GetTreeObject(path);
            ObjectNumber++;

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

        public bool RemoveTreeObject(string name)
        {
            var path = GetPath(name);
            var treeObject = GetTreeObject(path);
            if (treeObject.Children.Count > 0) return false;
            treeObject.Parent.RemoveChildren(treeObject);
            if (treeObject == CWD) CWD = null;
            ObjectNumber--;
            return true;
        }

        public bool Check(string name)
        {
            if (CWD == null)
            {
                Console.WriteLine("No such directory");
                return false;
            }

            if (ObjectNumber >= _maxDescriptorsNumber)
            {
                Console.WriteLine(
                    "Cannot create new file. Max number of objects in system has reached.");
                return false;
            }

            if (_maxFileNameLength < name.Length)
            {
                Console.WriteLine(
                    "Cannot create new object. The name is too long.");
                return false;
            }

            return true;
        }

    }
}
