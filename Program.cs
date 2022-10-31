using System;

namespace FileSystem
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var fs = new FileSystem(100);

            #region lab4

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
            fh.Read(10);
            fs.Ls();

            #endregion lab4


            Console.WriteLine();

            fs.MakeDir("dir1");
            fs.ShowStat("dir1");
            fs.Ls();

            Console.WriteLine();
            fs.MakeDir("dir1/dir2");
            fs.ShowStat("dir1");
            fs.Ls();
            fs.ShowStat("/");

            Console.WriteLine();
            fs.Cd("dir1/dir2");
            fs.Ls();
            fs.CreateFile("file.txt");
            fs.MakeDir("/a/b");
            fs.Ls();
            fs.Ls("/");

            Console.WriteLine();
            fs.Symlink("/dir1", "/a/b/l1");
            fs.Ls("/a/b");

            Console.WriteLine();
            fs.OpenFile("/a/b/l1/dir2/file.txt");
            fs.ShowStat("./../../dir1/././dir2/file.txt");
            fs.Symlink("./../../dir1/././dir2/file.txt", "/dir1/l2");
            fs.Cd("/a/b");
            fs.OpenFile("l1/l2");

            Console.WriteLine();
            fs.Cd("/");
            fs.Unlink("/dir1/l2");
            fs.Unlink("/a/b/l1");
            fs.Unlink("dir1/dir2/file.txt");
            fs.Unlink("/dir1/dir2");

            Console.WriteLine();
            fs.Symlink("some", "dir1/dir2/data.txt");
            fs.Ls("dir1/dir2");
            fs.Link("/dir1/dir2/data.txt", "/a/b/document");
            fs.Ls("/a/b");
            fs.ShowStat("/a/b/document");
            fs.ShowStat("/dir1/dir2/data.txt");

            Console.WriteLine();
            fs.OpenFile("/dir1/dir2/data.txt");
            fs.CreateFile("/some");
            fs.OpenFile("/dir1/dir2/data.txt");

            Console.WriteLine();
            fs.MakeDir("/1");
            fs.MakeDir("/1/2");
            fs.Cd("/1/2");
            fs.RmDir("../2");
            fs.Ls();
            fs.CreateFile("file.txt");
        }
    }
}
