using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCMD.Resources.Connection.PartialClasses
{
    public partial class Directory : IFileSystemObject
    {
        public Directory Parent { get => Directory2; set => Directory2 = value; }

        public IEnumerable<Directory> SubDirectories => Directory1;

        public IEnumerable<IFileSystemObject> Children
            => SubDirectories.Cast<IFileSystemObject>()
                             .Concat(Files.Cast<IFileSystemObject>());

        public byte[] IconOrDefault
        {
            get
            {
                if (Icon != null)
                    return Icon.Data;
                return DefaultIcons.DirectoryIcon;
            }
        }
    }
}
