using System.Linq;

namespace ConsoleCMD.Resources.Connection
{
    public partial class File : IFileSystemObject
    {
        public Directory Parent { get => Directory; set => Directory = value; }

        public byte[] IconOrDefault {
            get
            {
                if (Extension.Icon != null)
                    return Extension.Icon.Data;
                return DatabaseContext.Entities.Icons.Local.First(icon => icon.Id == 2).Data;
            }
        }

        public string FullName => $"{Name}.{Extension.Name}";
    }
}
