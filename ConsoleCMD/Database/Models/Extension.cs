using System;
using System.Collections.Generic;

namespace ConsoleCMD.Database.Models;

public partial class Extension
{
    public int Id { get; set; }

    public int? IconId { get; set; }

    public string Name { get; set; }

    public virtual ICollection<File> Files { get; } = new List<File>();

    public virtual Icon Icon { get; set; }
}
