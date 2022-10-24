using System;
using System.Collections.Generic;
using System.Linq;
using FileSystem.Descriptors;

namespace FileSystem
{
    internal class FileSystem
    {
        private readonly List<ObjectDescriptor> _descriptors;
        private readonly int _maxDescriptorsNumber;
        private readonly int _maxFileNameLength = 128;

        public DirDescriptor CWD;

        public FileSystem(int maxDescrNumber)
        {
            _maxDescriptorsNumber = maxDescrNumber;
            var rootDir = new DirDescriptor("/", "/", null);
            _descriptors = new List<ObjectDescriptor> { rootDir };
            
            CWD = rootDir;
            Console.WriteLine("The file system was created");
        }

        private ObjectDescriptor GetDescriptorByPath(string path)
        {
            foreach (var obj in _descriptors)
            {
                if (obj is FileDescriptor fileDescriptor)
                {
                    if (fileDescriptor.Links.Contains(path)) return obj;
                } else
                {
                    if (obj.Path == path) return obj;
                }
            }

            throw new Exception("No such object in file system"); // replace
        }

        private string GetPath(string name)
        {
            if (name.StartsWith('/')) return name;
            return CWD.Path == "/" ? $"/{name}" : $"{CWD.Path}/{name}";
        }

        private string GetNameFromPath(string path)
        {
            //Console.WriteLine($"GetNameFromPath: {path.Substring(path.LastIndexOf('/') + 1)}");
            return path.Substring(path.LastIndexOf('/') + 1);
        }

        public void MakeDir(string name)
        {
            var path = GetPath(name);
            if (_descriptors.Count >= _maxDescriptorsNumber)
            {
                Console.WriteLine(
                    "Cannot create new directory. Max number of objects in system has reached.");
                return;
            }

            try
            {
                var dir = new DirDescriptor(name, path, CWD);
                _descriptors.Add(dir);
            }
            catch (ArgumentException)
            {
                Console.WriteLine(
                    $"The directory {name} has been already created");
                return;
            }

            Console.WriteLine($"The directory {name} was created");
        }

        public void CreateFile(string name)
        {
            var path = GetPath(name);
            if (_maxFileNameLength < name.Length)
            {
                Console.WriteLine(
                    "Cannot create new file. Too long file name.");
                return;
            }

            if (_descriptors.Count >= _maxDescriptorsNumber)
            {
                Console.WriteLine(
                    "Cannot create new file. Max number of objects in system has reached.");
                return;
            }

            try
            {
                var descriptor = new FileDescriptor(name, path);
                _descriptors.Add(descriptor);
                FileHandler.CreateFile(descriptor);
            }
            catch (ArgumentException)
            {
                Console.WriteLine($"The file {name} has been already created");
                return;
            }

            Console.WriteLine($"The file {name} was created");
        }

        public void Ls(string name = null)
        {
            var dirname = name == null ? CWD.Path : GetPath(name);
            dirname = dirname == "/" ? "" : dirname; // fix ls for root directory

            Console.WriteLine($"List of objects in directory {dirname}");
            Console.WriteLine($"{CWD.Type},{CWD.Id}   =>   .");
            Console.WriteLine($"{CWD.ParentDir.Type},{CWD.ParentDir.Id}   =>   ..");

            foreach (var obj in _descriptors)
            {
                if (obj is FileDescriptor fileDescriptor)
                {
                    foreach (var link in fileDescriptor.Links)
                    {
                        var objName = GetNameFromPath(link);
                        if (link == $"{dirname}/{objName}")
                            Console.WriteLine($"{obj.Type},{obj.Id}   =>   {objName}");
                    }
                }
                else
                {
                    //Console.WriteLine($"LS for {GetNameFromPath(obj.Path)}: {dirname}/{GetNameFromPath(obj.Path)}");
                    var objName = GetNameFromPath(obj.Path);
                    if (objName != "" && obj.Path == $"{dirname}/{objName}")
                        Console.WriteLine($"{obj.Type},{obj.Id}   =>   {objName}");
                }
            }
        }

        public void ShowStat(string name)
        {
            var path = GetPath(name);
            try
            {
                var descriptor = GetDescriptorByPath(path);
                Console.WriteLine($"Information for {path}:\n    {descriptor.Stat()}");
            }
            catch (Exception)
            {
                Console.WriteLine($"No object with the name {name} in system");
            }
        }

        public void Link(string name1, string name2)
        {
            var path1 = GetPath(name1);
            var descriptor = (FileDescriptor)GetDescriptorByPath(path1);
            var path2 = GetPath(name2);
            descriptor.AddLink(path2);
            Console.WriteLine($"The link {name2} was created");
        }

        public void Unlink(string name)
        {
            var path = GetPath(name);
            var descriptor = (FileDescriptor)GetDescriptorByPath(path);
            descriptor.RemoveLink(path);
            if (descriptor.CanBeRemoved())
            {
                FileHandler.RemoveFile(descriptor);
                _descriptors.Remove(descriptor);
            }

            Console.WriteLine($"The file {name} was unlinked");
        }

        public void Truncate(string name, int size)
        {
            var path = GetPath(name);
            ((FileDescriptor)GetDescriptorByPath(path)).Truncate(size);
            Console.WriteLine($"The size of file {name} was changed");
        }

        public FileHandler OpenFile(string name)
        {
            var path = GetPath(name);
            try
            {
                var descriptor = (FileDescriptor)GetDescriptorByPath(path);
                var fd = new FileHandler(descriptor);
                Console.WriteLine(
                    $"The file {name} was opened, id of descriptor = {descriptor.Id}");
                return fd;
            }
            catch (Exception)
            {
                Console.WriteLine("No empty file handlers");
                return null;
            }
        }

        public DirDescriptor Cd(string name)
        {
            var path = GetPath(name);
            var dir = GetDescriptorByPath(path);
            CWD = (DirDescriptor)dir;
            Console.WriteLine($"Change CWD to {path}");
            return CWD;
        }
    }
}
