using System;
using System.Collections.Generic;
using FileSystem.Descriptors;

namespace FileSystem
{
    internal class FileSystem
    {
        private readonly Dictionary<string, ObjectDescriptor> _descriptors;
        private readonly int _maxDescriptorsNumber;
        private readonly int _maxFileNameLength = 128;

        public DirDescriptor CWD;

        public FileSystem(int maxDescrNumber)
        {
            _maxDescriptorsNumber = maxDescrNumber;
            _descriptors = new Dictionary<string, ObjectDescriptor>
            {
                {
                    ".",
                    new DirDescriptor(".", ".") // add default root directory
                }
            };

            CWD = (DirDescriptor)_descriptors["."];
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
                _descriptors.Add(path, new DirDescriptor(name, path));
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
                _descriptors.Add(path, descriptor);
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
            foreach (var path in _descriptors.Keys)
                if (path.StartsWith(dirname))
                {
                    var obj = _descriptors[path];
                    Console.WriteLine($"{obj.Type},{obj.Id}   =>   {path}");
                }
        }

        public void ShowStat(string name)
        {
            var path = $"{CWD.Name}/{name}";
            try
            {
                Console.WriteLine(
                    $"Information for {path}:\n    {_descriptors[path].Stat()}");
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine($"No file with the name {name} in system");
            }
        }

        public void Link(string name1, string name2)
        {
            var path1 = $"{CWD.Name}/{name1}";
            var file = (FileDescriptor)_descriptors[path1];
            var path2 = $"{CWD.Name}/{name2}";
            _descriptors.Add(path2, file);
            file.IncreaseNlink();
            Console.WriteLine($"The link {name2} was created");
        }

        public void Unlink(string name)
        {
            var path = $"{CWD.Name}/{name}";
            var descriptor = (FileDescriptor)_descriptors[path];
            descriptor.DecreaseNlink();
            if (descriptor.CanBeRemoved()) FileHandler.RemoveFile(descriptor);
            _descriptors.Remove(path);
            Console.WriteLine($"The file {name} was unlinked");
        }

        public void Truncate(string name, int size)
        {
            var path = $"{CWD.Name}/{name}";
            ((FileDescriptor)_descriptors[path]).Truncate(size);
            Console.WriteLine($"The size of file {name} was changed");
        }

        public int OpenFile(string name)
        {
            var path = $"{CWD.Name}/{name}";
            try
            {
                var fd = new FileHandler((FileDescriptor)_descriptors[path]);
                Console.WriteLine(
                    $"The file {name} was opened with fd = {fd.Id}");
                return fd.Id;
            }
            catch (Exception)
            {
                Console.WriteLine("No empty file handlers");
                return -1;
            }
        }

        public void CloseFile(int fd)
        {
            var fileHandler = FileHandler.GetFileHandlerById(fd);
            if (fileHandler == null)
            {
                Console.WriteLine($"No open file with fd = {fd}");
                return;
            }

            fileHandler.CloseFile(fd);
            Console.WriteLine($"The file with fd = {fd} was closed");
        }

        public void Seek(int fd, int offset)
        {
            var fileHandler = FileHandler.GetFileHandlerById(fd);
            if (fileHandler == null)
            {
                Console.WriteLine($"No open file with fd = {fd}");
                return;
            }

            fileHandler.Seek(offset);
            Console.WriteLine(
                $"The offset of file with fd = {fd} was moved to {offset}");
        }

        public void Read(int fd, int size)
        {
            var fileHandler = FileHandler.GetFileHandlerById(fd);
            if (fileHandler == null)
            {
                Console.WriteLine($"No open file with fd = {fd}");
                return;
            }

            var result = fileHandler.Read(size);
            Console.WriteLine($"The file with fd = {fd} was read: {result}");
        }

        public void Write(int fd, int size, string str)
        {
            var fileHandler = FileHandler.GetFileHandlerById(fd);
            if (fileHandler == null)
            {
                Console.WriteLine($"No open file with fd = {fd}");
                return;
            }

            fileHandler.Write(size, str);
            Console.WriteLine($"To the file with fd = {fd} was written: {str}");
        }
    }
}
