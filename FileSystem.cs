using System;
using System.Linq;
using FileSystem.Descriptors;
using FileSystem.Tree;

namespace FileSystem
{
    internal class FileSystem
    {
        private readonly FileTree _tree;

        public FileSystem(int maxDescrNumber)
        {
            _tree = new FileTree(new DirDescriptor("/", null), maxDescrNumber);
            Console.WriteLine("The file system was created");
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

        #region file methods

        public FileDescriptor CreateFile(string name)
        {
            if (!_tree.CanObjectBeCreated(name)) return null;
            var path = _tree.GetPath(name);

            if (_tree.GetObjectDescriptor(name) is not FileDescriptor descriptor)
            {
                var file = new FileDescriptor(path);
                _tree.AddTreeObject(file);
                FileHandler.CreateFile(file);
                Console.WriteLine($"The file {name} was created");
                return file;
            }

            if (descriptor.Created == false)
            {
                descriptor.Created = true;
                FileHandler.CreateFile(descriptor);
                Console.WriteLine($"The file {name} was created");
                return descriptor;
            }

            Console.WriteLine($"The file {name} has been already created");
            return descriptor;
        }

        public void Link(string name1, string name2)
        {
            var descriptor = _tree.GetObjectDescriptor(name1);
            var path2 = _tree.GetPath(name2);
            switch (descriptor)
            {
                case DirDescriptor:
                    Console.WriteLine("Link for directories is not allowed");
                    return;
                case SymLinkDescriptor:
                    _tree.AddTreeObject(descriptor, path2);
                    break;
            }

            descriptor.AddLink(path2);
            Console.WriteLine($"The link {name2} was created");
        }

        public void Unlink(string name)
        {
            var path = _tree.GetPath(name);
            var descriptor = _tree.GetObjectDescriptor(name);
            switch (descriptor)
            {
                case DirDescriptor:
                    Console.WriteLine("Unlink for directories is not allowed");
                    return;
                case FileDescriptor fileDescriptor:
                    if (fileDescriptor.CanBeRemoved())
                    {
                        FileHandler.RemoveFile(fileDescriptor);
                        _tree.RemoveTreeObject(descriptor);
                    }
                    fileDescriptor.RemoveLink(path);
                    break;
                case SymLinkDescriptor symLinkDescriptor:
                    _tree.RemoveTreeObject(descriptor);
                    symLinkDescriptor.RemoveLink(path);
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
            var id = FileHandler.GetId();
            if (id == -1)
            {
                Console.WriteLine("No empty file handlers");
                return null;
            }

            FileHandler fd = null;
            switch (_tree.GetObjectDescriptor(name))
            {
                case SymLinkDescriptor desc:
                    if (desc.LinkedObject is FileDescriptor { Created: true } fileDescriptor)
                    {
                        fd = new FileHandler(fileDescriptor, id);
                        Console.WriteLine($"The file {name} was opened");
                    }
                    else Console.WriteLine("No such object");
                    break;
                case FileDescriptor fileDesc:
                    fd = new FileHandler(fileDesc, id);
                    Console.WriteLine($"The file {name} was opened");
                    break;
            }
            return fd;
        }

        #endregion

        #region dir methods

        public void Ls(string name = null)
        {
            if (_tree.CWD == null)
            {
                Console.WriteLine("No such directory");
                return;
            }

            var dirname = name == null
                ? _tree.CWD.Descriptor.Path
                : _tree.GetPath(name);

            var treeObject = _tree.GetTreeObject(dirname);
            var descriptor = treeObject.Descriptor;
            if (treeObject.Descriptor is SymLinkDescriptor symLinkDescriptor)
            {
                descriptor = symLinkDescriptor.LinkedObject;
                dirname = symLinkDescriptor.LinkedObject.Path;
            }

            Console.WriteLine($"List of objects in directory {dirname}");
            Console.WriteLine($"{descriptor.Type},{descriptor.Id}   =>   .");
            Console.WriteLine($"{treeObject.Parent.Descriptor.Type},{treeObject.Parent.Descriptor.Id}   =>   ..");

            foreach (var obj in _tree.Ls(treeObject))
            foreach (var link in obj.Links.Where(link => link.StartsWith(dirname)))
                Console.WriteLine($"{obj.GetType()}   =>   {obj.GetNameFromPath(link)}");
        }

        public void MakeDir(string name)
        {
            if (!_tree.CanObjectBeCreated(name)) return;
            var pathCWD = _tree.CWD.Descriptor;
            if (name.StartsWith('/')) _tree.Cd(_tree.GetObjectDescriptor("/"));
            var names = name.Split('/');
            foreach (var dirName in names)
            {
                _tree.Cd(CreateDir(dirName));
            }
            _tree.Cd(pathCWD);
        }

        private DirDescriptor CreateDir(string name)
        {
            if (!_tree.CanObjectBeCreated(name)) return null;
            var path = _tree.GetPath(name);
            var dir = (DirDescriptor)_tree.GetObjectDescriptor(name);
            if (dir != null) return dir;
            dir = new DirDescriptor(path, (DirDescriptor)_tree.CWD.Descriptor);
            _tree.AddTreeObject(dir);
            Console.WriteLine($"The directory {name} was created");
            return dir;
        }

        public void RmDir(string name)
        {
            if (!_tree.CanObjectBeCreated(name)) return;
            if (_tree.RemoveTreeObject(_tree.GetObjectDescriptor(name)))
                Console.WriteLine($"The directory {name} was deleted");
            else Console.WriteLine($"The directory {name} can not be deleted");
        }

        public void Cd(string name)
        {
            _tree.Cd(_tree.GetObjectDescriptor(name));
            Console.WriteLine($"Change CWD to {_tree.CWD.Descriptor.Path}");
        }

        public void Symlink(string objectName, string pathname)
        {
            if (!_tree.CanObjectBeCreated(pathname)) return;
            var path = _tree.GetPath(pathname);
            var obj = _tree.GetObjectDescriptor(objectName);
            if (obj == null)
            {
                obj = new FileDescriptor(_tree.GetPath(objectName), false);
                _tree.AddTreeObject(obj);
            }
            if (obj is SymLinkDescriptor symLinkDescriptor)
            {
                obj = symLinkDescriptor.LinkedObject;
            }
            _tree.AddTreeObject(new SymLinkDescriptor(path, obj));
            Console.WriteLine($"Create symlink {pathname} for {objectName}");
        }

        #endregion
    }
}
