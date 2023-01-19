using System.Data.Entity;

namespace ConsoleCMD.Resources.Connection
{
    public static class DatabaseContext
    {
        public static Entities Entities { get; }

        static DatabaseContext()
        {
            Entities = new Entities();

            Entities.Icons.Load();
            Entities.Directories.Load();
            Entities.Extensions.Load();
            Entities.Files.Load();
        }
    }
}
