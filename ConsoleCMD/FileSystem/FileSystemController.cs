using ConsoleCMD.Resources.Connection;
using System.Linq;

namespace ConsoleCMD.FileSystem
{
    public static class FileSystemController
    {
        public static Directory Root { get; }

        static FileSystemController() =>
            Root = DatabaseContext.Entities.Directory.Local.First(directory => directory.Parent == null);

    }
}
