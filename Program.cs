using System;

namespace FileSystem
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var fs = new FileSystem(10);

            Console.WriteLine();
            var filename = "file.txt";
            fs.CreateFile(filename);
            fs.ShowStat(filename);

            Console.WriteLine();
            fs.Ls();

            Console.WriteLine();
            var linkname = "document.txt";
            fs.Link(filename, linkname);
            fs.Ls();
            fs.ShowStat(linkname);

            Console.WriteLine();
            var filename2 = "some.dat";
            fs.CreateFile(filename2);
            fs.Ls();

            Console.WriteLine();
            fs.Truncate(filename2, 1024);
            fs.ShowStat(filename2);

            Console.WriteLine();
            var fd = fs.OpenFile(filename2);

            fs.Write(fd, 10, "0123456789");
            fs.ShowStat(filename2);

            fs.Seek(fd, 7);
            fs.Read(fd, 2);

            Console.WriteLine();
            fs.Seek(fd, 256);
            fs.Write(fd, 6, "abcdfg");

            Console.WriteLine();
            fs.Seek(fd, 0);
            fs.Read(fd, 384);
            fs.ShowStat(filename2);

            Console.WriteLine();
            fs.Unlink(filename2);
            fs.Ls();

            Console.WriteLine();
            fs.Seek(fd, 0);
            fs.Read(fd, 10);

            Console.WriteLine();
            fs.CloseFile(fd);
        }
    }
}
