using System.Linq;

namespace ConsoleCMD.Resources.Connection
{
    public static class Icons
    {
        public static readonly byte[] DirectoryDefaultIcon = DatabaseContext.Entities.Icon.Local.First(icon => icon.ID == (int)DefaultIcon.Directory).Data;
        public static readonly byte[] FileDefaultIcon = DatabaseContext.Entities.Icon.Local.First(icon => icon.ID == (int)DefaultIcon.File).Data;
        public static readonly byte[] ApplicationDefaultIcon = DatabaseContext.Entities.Icon.Local.First(icon => icon.ID == (int)DefaultIcon.Application).Data;
    }

    public enum DefaultIcon
    {
        Directory = 1,
        File,
        Application
    }
}
