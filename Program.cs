using System;

namespace FileSystem
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var fs = new FileSystem(100);
            
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
            var fileHandler = fs.OpenFile(filename2);

            fileHandler.Write(10, "0123456789");
            fs.ShowStat(filename2);

            fileHandler.Seek(7);
            fileHandler.Read(2);

            Console.WriteLine();
            fileHandler.Seek(256);
            fileHandler.Write(7, "abcdefg");

            Console.WriteLine();
            fileHandler.Seek(0);
            fileHandler.Read(384);
            fs.ShowStat(filename2);

            Console.WriteLine();
            fs.Unlink(filename2);
            fs.Ls();

            Console.WriteLine();
            fileHandler.Seek(0);
            fileHandler.Read(10);

            Console.WriteLine();
            fileHandler.CloseFile();


            
            Console.WriteLine();
            fs.CreateFile("file1.txt");
            fs.Link("file1.txt", "document1.txt");
            var fh = fs.OpenFile("file1.txt");
            fh.Write(10, "0123456789");
            fh.CloseFile();
            fs.Ls();
            fs.Unlink("file1.txt");
            fs.Ls();
            fs.CreateFile("file1.txt");
            fh = fs.OpenFile("file1.txt");
            fh.Write(10, "abcdefghij");
            fh.CloseFile();
            fh = fs.OpenFile("document1.txt");
            fh.Read( 10);
            fs.Ls();
            

            Console.WriteLine();

            fs.MakeDir("dir1");
            fs.ShowStat("dir1");
            fs.Ls();

            fs.MakeDir("dir1/dir2");
            fs.ShowStat("dir1");
            fs.Ls();
            fs.ShowStat("/");
            
        }
    }
}
