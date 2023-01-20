using System;
using System.Collections.Generic;

namespace ConsoleCMD.Database.Models;

public partial class File : IFileSystemObject
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string FullName => $"{Name}.{Extension.Name}";

    public int ExtensionId { get; set; }

    public int ParentId { get; set; }

    public virtual Extension Extension { get; set; }

    public byte[] IconOrDefault
        => (Extension.Icon != null) ? Extension.Icon.Data : DefaultIcons.FileIcon.Data;

    public virtual Directory Parent { get; set; }
}
