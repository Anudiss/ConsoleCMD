using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCMD.Resources.Connection
{
    public partial class File : IFileSystemObject
    {
        public Directory Parent => Directory;

        public byte[] IconOrDefault {
            get
            {
                if (Icon != null)
                    return Icon.Data;
                return DatabaseContext.Entities.Icons.Local.First(icon => icon.Id == 2).Data;
            }
        }
    }
}
