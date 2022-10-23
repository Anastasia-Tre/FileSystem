using System;
using System.Collections.Generic;
using System.Linq;
using FileSystem.Descriptors;

namespace FileSystem
{
    internal class FileSystem
    {
        private readonly Dictionary<int, ObjectDescriptor> _descriptors;
        private readonly int _maxDescriptorsNumber;
        private readonly int _maxFileNameLength = 128;

        public DirDescriptor CWD;

        public FileSystem(int maxDescrNumber)
        {
            _maxDescriptorsNumber = maxDescrNumber;
            var rootDir = new DirDescriptor(".", ".");
            _descriptors = new Dictionary<int, ObjectDescriptor>
            {
                {
                    rootDir.Id,
                    rootDir
                }
            };

            //CWD = (DirDescriptor)_descriptors[rootDir.Id];
            CWD = rootDir;
            Console.WriteLine("The file system was created");
        }

        public void MakeDir(string name)
        {
            var path = $"{CWD.Name}/{name}";
            if (_descriptors.Count >= _maxDescriptorsNumber)
            {
                Console.WriteLine(
                    "Cannot create new directory. Max number of objects in system has reached.");
                return;
            }

            try
            {
                var dir = new DirDescriptor(name, path);
                _descriptors.Add(dir.Id, dir);
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
            var path = $"{CWD.Name}/{name}";
            if (_maxFileNameLength < name.Length)
            {
                Console.WriteLine(
                    "Cannot create new directory. Too long file name.");
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
                _descriptors.Add(descriptor.Id, descriptor);
                FileHandler.CreateFile(descriptor);
            }
            catch (ArgumentException)
            {
                Console.WriteLine($"The file {name} has been already created");
                return;
            }

            Console.WriteLine($"The file {name} was created");
        }

        public void Ls(string name = "")
        {
            var dirname = $"{CWD.Name}/{name}";
            Console.WriteLine($"List of objects in directory {dirname}");
            foreach (var obj in _descriptors.Values)
            {
                foreach (var link in obj.Links)
                {
                    if (link.StartsWith(dirname))
                    {
                        Console.WriteLine(
                            $"{obj.Type},{obj.Id}   =>   {link}");
                    }
                }
            }
        }

        public void ShowStat(string name)
        {
            var path = $"{CWD.Name}/{name}";
            try
            {
                var descriptor = GetDescriptorByPath(path);
                Console.WriteLine(
                    $"Information for {path}:\n    {descriptor.Stat()}");
            }
            catch (Exception)
            {
                Console.WriteLine($"No file with the name {name} in system");
            }
        }

        private ObjectDescriptor GetDescriptorByPath(string path)
        {
            return _descriptors.Values.First(obj => obj.Links.Contains(path));
        }

        public void Link(string name1, string name2)
        {
            var path1 = $"{CWD.Name}/{name1}";
            var descriptor = (FileDescriptor)GetDescriptorByPath(path1);
            var path2 = $"{CWD.Name}/{name2}";
            descriptor.AddLink(path2);
            Console.WriteLine($"The link {name2} was created");
        }

        public void Unlink(string name)
        {
            var path = $"{CWD.Name}/{name}";
            var descriptor = (FileDescriptor)GetDescriptorByPath(path);
            descriptor.RemoveLink(path);
            if (descriptor.CanBeRemoved())
            {
                FileHandler.RemoveFile(descriptor);
                _descriptors.Remove(descriptor.Id);
            }
            
            Console.WriteLine($"The file {name} was unlinked");
        }

        public void Truncate(string name, int size)
        {
            var path = $"{CWD.Name}/{name}";
            ((FileDescriptor)GetDescriptorByPath(path)).Truncate(size);
            Console.WriteLine($"The size of file {name} was changed");
        }

        public FileHandler OpenFile(string name)
        {
            var path = $"{CWD.Name}/{name}";
            try
            {
                var descriptor = (FileDescriptor)GetDescriptorByPath(path);
                var fd = new FileHandler(descriptor);
                Console.WriteLine($"The file {name} was opened, id of descriptor = {descriptor.Id}");
                return fd;
            }
            catch (Exception)
            {
                Console.WriteLine("No empty file handlers");
                return null;
            }
        }
    }
}
