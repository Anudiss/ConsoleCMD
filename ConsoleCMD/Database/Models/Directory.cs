using System.Collections.Generic;
using System.Linq;

namespace ConsoleCMD.Database.Models;

public partial class Directory : IFileSystemObject
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int? ParentId { get; set; }

    public int? IconId { get; set; }

    public virtual Icon Icon { get; set; }

    public byte[] IconOrDefault
        => (Icon != null) ? Icon.Data : DefaultIcons.DirectoryIcon.Data;

    public virtual Directory Parent { get; set; }
    
    public virtual ICollection<File> Files { get; } = new List<File>();

    public virtual ICollection<Directory> SubDirectories { get; } = new List<Directory>();

    public IEnumerable<IFileSystemObject> Children
        => Files.Cast<IFileSystemObject>().Concat(SubDirectories);
}
