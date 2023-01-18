using System.Data.Entity;

namespace ConsoleCMD.Resources.Connection
{
    public static class DatabaseContext
    {
        public static FileSystemEntities Entities { get; }

        static DatabaseContext()
        {
            Entities = new FileSystemEntities();

            Entities.Icons.Load();
            Entities.Directories.Load();
            Entities.Extensions.Load();
            Entities.Files.Load();
        }
    }
}
