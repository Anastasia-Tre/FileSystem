using System;

namespace FileSystem
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var fs = new FileSystem(10);
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

            var filename2 = "some.dat";
            fs.CreateFile(filename2);
            fs.Ls();

            Console.WriteLine();
            fs.Truncate(filename2, 1024);
            fs.ShowStat(filename2);


            Console.WriteLine();
            var fileHandler = fs.OpenFile(filename2);

            fileHandler.Write(10, "0123456789");
            fs.ShowStat(filename2);

            fileHandler.Seek(7);
            fileHandler.Read(2);

            fileHandler.Seek(256);
            fileHandler.Write(7, "abcdfg");

            fileHandler.Seek(0);
            fileHandler.Read(384);
            fs.ShowStat(filename2);


            Console.WriteLine();
            fs.Unlink(filename2);
            fs.Ls();
            fileHandler.Seek(0);
            fileHandler.Read(10);

            fileHandler.CloseFile();

        }
    }
}
