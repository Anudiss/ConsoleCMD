using System;
using System.Collections.Generic;

namespace ConsoleCMD.Database.Models;

public partial class Icon
{
    public int Id { get; set; }

    public byte[] Data { get; set; }

    public virtual ICollection<Directory> Directories { get; } = new List<Directory>();

    public virtual ICollection<Extension> Extensions { get; } = new List<Extension>();
}
