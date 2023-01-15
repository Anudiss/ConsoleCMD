using System.Linq;

namespace ConsoleCMD.Resources.Connection
{
    public partial class File : IFileSystemObject
    {
        public Directory Parent => Directory;

        public byte[] IconOrDefault {
            get
            {
                if (Extension.Icon != null)
                    return Extension.Icon.Data;
                return DatabaseContext.Entities.Icons.Local.First(icon => icon.Id == 2).Data;
            }
        }
    }
}
