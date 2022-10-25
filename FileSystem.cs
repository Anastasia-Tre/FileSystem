﻿using System;
using FileSystem.Descriptors;
using FileSystem.Tree;

namespace FileSystem
{
    internal class FileSystem
    {
        private readonly FileTree _tree;
        private readonly int _maxDescriptorsNumber;
        private readonly int _maxFileNameLength = 128;

        public FileSystem(int maxDescrNumber)
        {
            _maxDescriptorsNumber = maxDescrNumber;
            _tree = new FileTree(new DirDescriptor("/", null));
            Console.WriteLine("The file system was created");
        }

        public void MakeDir(string name) // rewrite, maybe move to FileTree
        {
            var tempCWD = _tree.CWD;
            if (name.StartsWith('/')) _tree.Cd("/");
            var names = name.Split('/');
            foreach (var dirName in names)
            {
                CreateDir(dirName);
                Cd(dirName);
            }
            _tree.CWD = tempCWD;
        }

        private DirDescriptor CreateDir(string name)
        {
            var path = _tree.GetPath(name);
            if (FileTree.ObjectNumber >= _maxDescriptorsNumber)
            {
                Console.WriteLine(
                    "Cannot create new directory. Max number of objects in system has reached.");
                return null;
            }

            var descriptor = _tree.GetObjectDescriptor(name);
            if (descriptor == null)
            {
                var dir = new DirDescriptor(path, (DirDescriptor)_tree.CWD.Descriptor);
                _tree.AddTreeObject(dir);
                Console.WriteLine($"The directory {name} was created");
                return dir;
            }
            Console.WriteLine($"The directory {name} has been already created");
            return (DirDescriptor)descriptor;
        }

        public FileDescriptor CreateFile(string name)
        {
            var path = _tree.GetPath(name);
            if (_maxFileNameLength < name.Length)
            {
                Console.WriteLine(
                    "Cannot create new file. Too long file name.");
                return null;
            }

            if (FileTree.ObjectNumber >= _maxDescriptorsNumber)
            {
                Console.WriteLine(
                    "Cannot create new file. Max number of objects in system has reached.");
                return null;
            }

            var descriptor = _tree.GetObjectDescriptor(name);
            if (descriptor == null)
            {
                var file = new FileDescriptor(path);
                _tree.AddTreeObject(file);
                FileHandler.CreateFile(file);
                Console.WriteLine($"The file {name} was created");
                return file;
            }
            Console.WriteLine($"The file {name} has been already created");
            return (FileDescriptor)descriptor;

        }

        public void Ls(string name = null) 
        {
            var dirname = name == null ? _tree.CWD.Descriptor.Path : _tree.GetPath(name);

            Console.WriteLine($"List of objects in directory {dirname}");
            Console.WriteLine($"{_tree.CWD.Descriptor.Type},{_tree.CWD.Descriptor.Id}   =>   .");
            Console.WriteLine($"{_tree.CWD.Parent.Descriptor.Type},{_tree.CWD.Parent.Descriptor.Id}   =>   ..");
            
            foreach (var obj in _tree.Ls(dirname))
            {
                if (obj is FileDescriptor fileDescriptor)
                {
                    foreach (var link in fileDescriptor.Links)
                    {
                        Console.WriteLine($"{obj.GetType()}   =>   {obj.GetNameFromPath(link)}");
                    }
                }
                else
                {
                    Console.WriteLine($"{obj.GetType()}   =>   {obj.Name}");
                }
            }
        }

        public void ShowStat(string name)
        {
            var descriptor = _tree.GetObjectDescriptor(name);
            if (descriptor != null)
            {
                Console.WriteLine($"Information for {name}:\n    {descriptor.Stat()}");
                return;
            }
            Console.WriteLine($"No object with the name {name} in system");
        }

        public void Link(string name1, string name2)
        {
            var descriptor = _tree.GetObjectDescriptor(name1);
            switch (descriptor)
            {
                case DirDescriptor:
                    Console.WriteLine($"Link for directories is not allowed");
                    return;
                case FileDescriptor fileDescriptor:
                {
                    var path2 = _tree.GetPath(name2);
                    fileDescriptor.AddLink(path2);
                    break;
                }
                case SymLinkDescriptor symLinkDescriptor:
                    break;
            }
            Console.WriteLine($"The link {name2} was created");
        }

        public void Unlink(string name)
        {
            var path = _tree.GetPath(name); // remove
            var descriptor = _tree.GetObjectDescriptor(name);
            switch (descriptor)
            {
                case DirDescriptor:
                    Console.WriteLine($"Unlink for directories is not allowed");
                    return;
                case FileDescriptor fileDescriptor:
                {
                    fileDescriptor.RemoveLink(path);
                    if (fileDescriptor.CanBeRemoved())
                    {
                        FileHandler.RemoveFile(fileDescriptor);
                        _tree.RemoveTreeObject(path);
                    }
                    break;
                }
                case SymLinkDescriptor symLinkDescriptor:
                    break;
            }

            Console.WriteLine($"The object {name} was unlinked");
        }

        public void Truncate(string name, int size)
        {
            ((FileDescriptor)_tree.GetObjectDescriptor(name)).Truncate(size);
            Console.WriteLine($"The size of file {name} was changed");
        }

        public FileHandler OpenFile(string name)
        {
            try
            {
                var fd = _tree.GetObjectDescriptor(name) switch
                {
                    SymLinkDescriptor desc => new FileHandler((FileDescriptor)desc.LinkedObject),
                    FileDescriptor fileDesc => new FileHandler(fileDesc),
                    _ => null
                };

                Console.WriteLine($"The file {name} was opened");
                return fd;
            }
            catch (Exception)
            {
                Console.WriteLine("No empty file handlers");
                return null;
            }
        }

        public void Cd(string name)
        {
            _tree.Cd(name);
            Console.WriteLine($"Change CWD to {_tree.CWD.Descriptor.Path}");
        }

        public void Symlink(string objectName, string pathname)
        {
            var path = _tree.GetPath(pathname);
            var obj = _tree.GetObjectDescriptor(objectName);
            _tree.AddTreeObject(new SymLinkDescriptor(path, obj));
        }
    }
}
